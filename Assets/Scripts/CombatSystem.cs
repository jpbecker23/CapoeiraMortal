using Unity.VisualScripting;
using UnityEngine;


public class CombatSystem : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    public GameObject Protagonista;
    public GameObject Vilao;

    void Start()
    {

    }

    void Update()
    {
        //playerController.HandleCombat();
        if (Input.GetKeyDown(KeyCode.K)) PerformAttack("Bencao");
        else if (Input.GetKeyDown(KeyCode.J)) PerformAttack("Armada");
        else if (Input.GetKeyDown(KeyCode.L)) PerformAttack("Chapa");
        else if (Input.GetKeyDown(KeyCode.U)) PerformAttack("Rasteira");
        else if (Input.GetKeyDown(KeyCode.I)) PerformAttack("Couro");
    }


    public void DoAttack()
    {
        Protagonista.GetComponent<Animator>().SetTrigger("Bencao");
    }

    public void TakeDamage()
    {
        
        if (Vector3.Distance(Protagonista.transform.position, Vilao.transform.position) < 2.0f)
        {
            Vilao.GetComponent<Animator>().SetTrigger("TakeDamage");
        }
    }

    public bool PerformAttack(string attackName, float damageMultiplier = 1f)
    {
        Protagonista.GetComponent<Animator>().SetTrigger(attackName);
        //Debug.Log($"Performing attack: {attackName} with damage multiplier: {damageMultiplier}");
        return true;
    }

}
