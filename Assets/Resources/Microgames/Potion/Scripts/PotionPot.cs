﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PotionPot : MonoBehaviour
{

	public int ingredientCount;
	public SpriteRenderer[] ingredientSlots;

	public Sprite[] ingredientSprites;
	public ParticleSystem[] potParticles, conditionalParticles;
	public ParticleSystem smokeParticles;
	public Vibrate potVibrate;
	public ObjectPool ingredientPool;
	public Transform ingredientSpawn;
	public GameObject victoryPotion;

	public Animator potAnimator, marisaAnimator;

	public Texture2D[] texturesToLoad;

	public State state;
	public enum State
	{
		Default,
		Victory,
		Failure
	}

	private static PotionIngredient[] allIngredients;

	private PotionIngredient[] ingredients, ingredientsNeeded;


	void Awake()
	{
		if (ingredientPool.addToPool(ingredientSprites.Length, true))
		{
			allIngredients = new PotionIngredient[ingredientSprites.Length];
			for (int i = 0; i < allIngredients.Length; allIngredients[i] = ingredientPool.getObjectWithoutUnpooling(i).GetComponent<PotionIngredient>(),  
			allIngredients[i].spriteRenderer.sprite = ingredientSprites[i], i++);
			

		}

		smokeParticles.playbackSpeed = 1.75f;

		reset();
	}

	void resetIngredients()
	{
		ingredients = new PotionIngredient[ingredientCount];
		int inglen = ingredients.Length; //always make a variable for your length, because finding the length takes O(n) time, but if you use this, it only takes O(1) time
		List<PotionIngredient> availableIngredients = new List<PotionIngredient>(allIngredients);
		int index;

		for (int i = 0; i < inglen; ingredients[i].pot = this, ingredients[i].name = "Ingredient " + i.ToString(), ingredients[i].state = PotionIngredient.State.Idle,  i++)
		{
			index = Random.Range(0, availableIngredients.Count);
			ingredients[i] = availableIngredients[index];
			availableIngredients.RemoveAt(index);
			ingredientPool.getObjectFromPool(ingredients[i].gameObject);

			//Transform spawn = ingredientSpawn.GetChild(Random.Range(0, ingredientSpawn.childCount));
			Transform spawn = ingredientSpawn.GetChild(i % ingredientSpawn.childCount);
			Vector3 position = spawn.position + new Vector3(Random.Range(-.5f * spawn.localScale.x, .5f * spawn.localScale.z), 0f, 0f);
			ingredients[i].transform.position = new Vector3(position.x, position.y, ingredients[i].transform.position.z);
		}
		
		int[] skiparray = new int[inglen];
		int skip = 0;
		for(int i = 0; i < inglen; i++){ //this takes O(n) time
		if (ingredients[i].theCollider.gameObject.activeSelf){
		skiparray[skip] = i;
		skip++;
		}
		}

		for (int i = 0; i < skip-1; i ++){
		int j = skip-1;
		while(j > i){
		Physics2D.IgnoreCollision(ingredients[skiparray[i]].theCollider, ingredients[skiparray[j]].theCollider, true);
		j--;
		}
		}
		int ingSlot = ingredientSlots.Length;
		availableIngredients = new List<PotionIngredient>(allIngredients);
		ingredientsNeeded = new PotionIngredient[ingSlot];
		for (int i = 0; i < ingSlot; ingredientSlots[i].color = Color.white, availableIngredients.RemoveAt(index),  i++ )
		{
			index = Random.Range(0, availableIngredients.Count);
			ingredientsNeeded[i] = availableIngredients[index];
			ingredientSlots[i].sprite = ingredientsNeeded[i].spriteRenderer.sprite;
		}

		orderIngredients();
	}
	
	

	public void moveToFront(PotionIngredient ingredient)
	{
		int index = 0;
		PotionIngredient hold = ingredient;
		for (int i = 0; i < ingredients.Length; i++)
		{
			if (ingredients[i] == ingredient)
			{
				index = i;
				i = ingredients.Length;
			}
		}

		for (int i = index - 1; i >= 0; ingredients[i + 1] = ingredients[i], i--);
		ingredients[0] = hold;
		orderIngredients();
	}

	void orderIngredients()
	{
		for (int i = 0; i < ingredients.Length;ingredients[i].spriteRenderer.sortingOrder = 60 - i, ingredients[i].transform.position = 
		new Vector3(ingredients[i].transform.position.x, ingredients[i].transform.position.y, -2f + (.01f * (float)i)), i++);
	}

	public void reset()
	{
		ingredientPool.poolAllObjects();
		potVibrate.vibrateOn = false;
		foreach (ParticleSystem particles in potParticles)
		{
			particles.Play();
		}
		resetIngredients();

		setState(State.Default);
		
	}



	void stopPotParticles()
	{
		foreach (ParticleSystem particles in potParticles)
		{
			particles.Stop();
		}
	}

	//void playContitionalParticles()
	//{
	//	foreach (ParticleSystem particles in conditionalParticles)
	//	{
	//		particles.Play();
	//	}
	//}

	void killContitionalParticles()
	{
		foreach (ParticleSystem particles in conditionalParticles)
		{
			particles.Stop();
			particles.SetParticles(new ParticleSystem.Particle[0], 0);
		}
	}

	void setState(State state)
	{
		this.state = state;
		potAnimator.SetInteger("state", (int)state);
		marisaAnimator.SetInteger("state", (int)state);

		switch(state)
		{
			case (State.Default):
				potVibrate.vibrateOn = false;
				//CameraController.instance.reset();
				killContitionalParticles();
				break;
			case(State.Victory):
				stopPotParticles();
				Invoke("awakenPotion", .4f);
				MicrogameController.instance.setVictory(true, true);
				break;
			case(State.Failure):
				stopPotParticles();
				MicrogameController.instance.setVictory(false, true);
				break;
			default:
				break;
		}
	}

	void awakenPotion()
	{
		victoryPotion.SetActive(true);
	}

	

	public void addIngredient(PotionIngredient ingredient)
	{
		for (int i = 0; i < ingredientsNeeded.Length; i++)
		{
			if (ingredientsNeeded[i] == ingredient)
			{
				ingredientSlots[i].color = new Color(.5f, .5f, .5f, .25f);

				bool done = true;
				for (int j = 0; j < ingredientsNeeded.Length; j++)
				{
					if (ingredientsNeeded[j].state != PotionIngredient.State.Used)
						done = false;
				}
				if (done)
					setState(State.Victory);
				return;
			}
		}
		setState(State.Failure);
	}


}
