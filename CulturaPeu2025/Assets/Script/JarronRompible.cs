using NUnit.Framework;
using UnityEngine;

public class JarronRompible : MonoBehaviour
{
    [SerializeField] private GameObject pickupObject;
   
    [SerializeField] private GameObject jarronParticle1;
    [SerializeField] private GameObject jarronParticle2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //item index= 
    //0: Latigo, 1: Mascara, 2: Chumpi, 3: Pututu Mully, 4: Conopas, 5: Tumi, 6: Hojas de coca, 7: Bolsa de objetos espirituales


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Weapon"))
        {
            // Play break sound
            AudioManager.instance.PlaySfx("PotBreak");
            // Instantiate the pickup object at the jarron's position
           
                Instantiate(pickupObject, transform.position, Quaternion.identity);
                
            
            Instantiate(jarronParticle1, transform.position, Quaternion.identity);
            Instantiate(jarronParticle2, transform.position, Quaternion.identity);
            // Destroy the jarron game object
            Destroy(gameObject);
        }
    }
}
