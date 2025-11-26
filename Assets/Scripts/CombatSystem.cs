using Unity.VisualScripting;
using UnityEngine;


public class CombatSystem : MonoBehaviour
{
    public float PlayerMaxHealth = 100;
    public float EnemyMaxHealth = 100;
    public float PlayerCurrentHealth;

    public float EnemyCurrentHealth;

    public PlayerHealthBar playerHealthBar;
    public EnemyHealthBar enemyHealthBar;
    [SerializeField] private PlayerController playerController;
    public GameObject Protagonista;
    public GameObject Vilao;

    void Start()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
        playerHealthBar.SetPlayerMaxHealth(PlayerMaxHealth);

        EnemyCurrentHealth = EnemyMaxHealth;
        enemyHealthBar.SetEnemyMaxHealth(EnemyMaxHealth);
    }

    void Update()
    {
        //playerController.HandleCombat();
        if (Input.GetKeyDown(KeyCode.K)) PerformAttack("Bencao", 10f);
        else if (Input.GetKeyDown(KeyCode.J)) PerformAttack("Armada", 10f);
        else if (Input.GetKeyDown(KeyCode.L)) PerformAttack("Chapa", 10f);
        else if (Input.GetKeyDown(KeyCode.U)) PerformAttack("Rasteira", 10f);
        else if (Input.GetKeyDown(KeyCode.I)) PerformAttack("Couro", 10f);
    }


    public void DoAttack()
    {
        Protagonista.GetComponent<Animator>().SetTrigger("Bencao");
    }

    public void TakeDamage(float damage)
    {

        if (Vector3.Distance(Protagonista.transform.position, Vilao.transform.position) < 2.0f)
        {
            Vilao.GetComponent<Animator>().SetTrigger("TakeDamage");
            EnemyCurrentHealth -= damage;
            enemyHealthBar.SetEnemyHealth(EnemyCurrentHealth);
        }
        else if (Vector3.Distance(Vilao.transform.position, Protagonista.transform.position) < 2.0f)
        {
            Protagonista.GetComponent<Animator>().SetTrigger("TakeDamage");
            PlayerCurrentHealth -= damage;
            playerHealthBar.SetPlayerHealth(PlayerCurrentHealth);
        }
    }

    public bool PerformAttack(string attackName, float damageMultiplier = 1f)
    {
        Protagonista.GetComponent<Animator>().SetTrigger(attackName);
        Vilao.GetComponent<Animator>().SetTrigger(attackName);
        TakeDamage(damageMultiplier);

        return true;
    }

}
