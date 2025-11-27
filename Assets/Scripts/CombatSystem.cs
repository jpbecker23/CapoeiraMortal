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
    [SerializeField] private EnemyAI enemyAI;
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
        if (Input.GetKeyDown(KeyCode.K)) PlayerPerformAttack("Bencao", 10f);
        else if (Input.GetKeyDown(KeyCode.J)) PlayerPerformAttack("Armada", 10f);
        else if (Input.GetKeyDown(KeyCode.L)) PlayerPerformAttack("Chapa", 10f);
        else if (Input.GetKeyDown(KeyCode.U)) PlayerPerformAttack("Rasteira", 10f);
        else if (Input.GetKeyDown(KeyCode.I)) PlayerPerformAttack("Couro", 10f);
    }

    public void PlayerTakeDamage(float damage)
    {
        if (Vector3.Distance(Vilao.transform.position, Protagonista.transform.position) < 2.0f)
        {
            Protagonista.GetComponent<Animator>().SetTrigger("TakeDamage");
            PlayerCurrentHealth -= damage;
            playerHealthBar.SetPlayerHealth(PlayerCurrentHealth);
        }
    }

    public void EnemyTakeDamage(float damage)
    {
        if (Vector3.Distance(Protagonista.transform.position, Vilao.transform.position) < 2.0f)
        {
            Vilao.GetComponent<Animator>().SetTrigger("TakeDamage");
            EnemyCurrentHealth -= damage;
            enemyHealthBar.SetEnemyHealth(EnemyCurrentHealth);
        }
    }

    public bool PlayerPerformAttack(string attackName, float damageMultiplier = 1f)
    {
        Protagonista.GetComponent<Animator>().SetTrigger(attackName);
        EnemyTakeDamage(damageMultiplier);
        return true;
    }

    public bool EnemyPerfomAttack(string attackName, float damageMultiplier = 1f)
    {
        Vilao.GetComponent<Animator>().SetTrigger(attackName);
        PlayerTakeDamage(damageMultiplier);
        return true;
    }

}
