using UnityEngine;
using System.Collections;

namespace Completed
{
	public class Wall : MonoBehaviour
	{
		public AudioClip chopSound1;				
		public AudioClip chopSound2;				
		public Sprite dmgSprite;					
		public int hp = 3;							
		
		
		private SpriteRenderer spriteRenderer;		
		
		
		void Awake ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
		}
		
		
		public void DamageWall (int loss)
		{
			SoundManager.instance.RandomizeSfx (chopSound1, chopSound2);
			
			spriteRenderer.sprite = dmgSprite;
			
			hp -= loss;
			
			if(hp <= 0)
				gameObject.SetActive (false);
		}
	}
}
