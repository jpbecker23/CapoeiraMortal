using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class CombatSystem : MonoBehaviour
{
    public float PlayerMaxHealth = 100;
    public float EnemyMaxHealth = 100;
    public float PlayerCurrentHealth;

    public float EnemyCurrentHealth;

    public UnityEvent nextLevelEvent;

    public PlayerHealthBar playerHealthBar;
    public EnemyHealthBar enemyHealthBar;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private EnemyAI enemyAI;
    public GameObject Protagonista;
    public GameObject Vilao;

    private bool playerIsAttacking = false;

    void Start()
    {
        PlayerCurrentHealth = PlayerMaxHealth;
        playerHealthBar.SetPlayerMaxHealth(PlayerMaxHealth);

        EnemyCurrentHealth = EnemyMaxHealth;
        enemyHealthBar.SetEnemyMaxHealth(EnemyMaxHealth);
    }

    void Update()
    {
        if (EnemyCurrentHealth <= 0)
        {
            Debug.Log("INIMIGO SEM VIDA");
            nextLevelEvent.Invoke();
        }

        if (!playerIsAttacking)
        {

            if (Input.GetKeyDown(KeyCode.K)) PlayerPerformAttack("Bencao", 10f);
            else if (Input.GetKeyDown(KeyCode.J)) PlayerPerformAttack("Armada", 10f);
            else if (Input.GetKeyDown(KeyCode.L)) PlayerPerformAttack("Chapa", 10f);
            else if (Input.GetKeyDown(KeyCode.I)) PlayerPerformAttack("Couro", 10f);
        }
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
            playerController.PlayerAudio.PlayOneShot(playerController.attackSound);
        }
    }

    public bool PlayerPerformAttack(string attackName, float damageMultiplier = 1f, float timeToEndAttack = 1.5f)
    {
        if (playerIsAttacking) return true;
        playerIsAttacking = true;
        Protagonista.GetComponent<Animator>().SetTrigger(attackName);
        //EnemyTakeDamage(damageMultiplier);
        Invoke("FinalizaAttackPlayer", timeToEndAttack);
        return true;
    }

    void FinalizaAttackPlayer()
    {
        playerIsAttacking = false;
    }

    public bool EnemyPerfomAttack(string attackName, float damageMultiplier = 1f)
    {
        if (playerIsAttacking) return false;
        Vilao.GetComponent<Animator>().SetTrigger(attackName);
        //PlayerTakeDamage(damageMultiplier);
        return true;
    }
}
