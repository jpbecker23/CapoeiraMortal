using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// CORREÇÃO COMPLETA PARA PRODUÇÃO - Script Editor Unity Unificado
/// Este script consolida TODAS as correções necessárias para o projeto:
/// - Valida e corrige Animator Controllers (estados, animações, transições)
/// - Remove scripts faltando (missing scripts)
/// - Configura NavMesh e posiciona personagens
/// - Corrige T-pose e garante animações funcionando
/// - Valida personagens na cena e seus componentes
/// Execute: Tools > Capoeira Mortal > CORREÇÃO COMPLETA PARA PRODUÇÃO
/// </summary>
public class CorrecaoCompletaProducao : EditorWindow
{
    private Vector2 scrollPos;
    private List<string> problemasEncontrados = new List<string>();
    private List<string> correcoesAplicadas = new List<string>();
    
    [MenuItem("Tools/Capoeira Mortal/CORREÇÃO COMPLETA PARA PRODUÇÃO")]
    public static void ShowWindow()
    {
        GetWindow<CorrecaoCompletaProducao>("Correção Completa para Produção");
    }
    
    void OnGUI()
    {
        GUILayout.Label("CORREÇÃO COMPLETA PARA PRODUÇÃO", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "Este script valida e corrige TODOS os aspectos do projeto:\n" +
            "1. Remove scripts faltando (missing scripts)\n" +
            "2. Valida e corrige Animator Controllers\n" +
            "3. Configura NavMesh e posiciona personagens\n" +
            "4. Corrige T-pose e garante animações funcionando\n" +
            "5. Valida personagens na cena e componentes\n" +
            "6. Força Animators a funcionarem corretamente",
            MessageType.Info);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("VALIDAR E CORRIGIR TUDO", GUILayout.Height(60)))
        {
            ValidarECorrigirTudo();
        }
        
        GUILayout.Space(20);
        
        if (problemasEncontrados.Count > 0 || correcoesAplicadas.Count > 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
            
            if (problemasEncontrados.Count > 0)
            {
                GUILayout.Label("Problemas Encontrados:", EditorStyles.boldLabel);
                foreach (string problema in problemasEncontrados)
                {
                    EditorGUILayout.HelpBox(problema, MessageType.Warning);
                }
            }
            
            if (correcoesAplicadas.Count > 0)
            {
                GUILayout.Label("Correções Aplicadas:", EditorStyles.boldLabel);
                foreach (string correcao in correcoesAplicadas)
                {
                    EditorGUILayout.HelpBox(correcao, MessageType.Info);
                }
            }
            
            EditorGUILayout.EndScrollView();
        }
    }
    
    /// <summary>
    /// Função principal que executa todas as correções em sequência
    /// </summary>
    void ValidarECorrigirTudo()
    {
        // Verificar se está em Play Mode
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Aviso", 
                "O script não pode ser executado durante o Play Mode!\n\n" +
                "Por favor:\n" +
                "1. Saia do Play Mode (clique no botão Play)\n" +
                "2. Execute o script novamente\n\n" +
                "Algumas correções podem funcionar, mas a cena não será salva.",
                "OK");
            Debug.LogWarning("========================================");
            Debug.LogWarning("AVISO: Script executado durante Play Mode!");
            Debug.LogWarning("Algumas operacoes podem nao funcionar corretamente.");
            Debug.LogWarning("Recomendado: Saia do Play Mode e execute novamente.");
            Debug.LogWarning("========================================");
        }
        
        problemasEncontrados.Clear();
        correcoesAplicadas.Clear();
        
        Debug.Log("========================================");
        Debug.Log("=== CORRECAO COMPLETA PARA PRODUCAO ===");
        Debug.Log("========================================");
        Debug.Log($"Iniciando validacao completa em: {System.DateTime.Now}");
        Debug.Log("");
        
        // ETAPA 1: Limpar scripts faltando
        Debug.Log(">>> ETAPA 1: LIMPANDO SCRIPTS FALTANDO <<<");
        LimparScriptsFaltando();
        Debug.Log("");
        
        // ETAPA 2: Validar e corrigir Animator Controllers
        Debug.Log(">>> ETAPA 2: VALIDANDO ANIMATOR CONTROLLERS <<<");
        ValidarECorrigirAnimatorControllers();
        Debug.Log("");
        
        // ETAPA 3: Validar e corrigir personagens na cena
        Debug.Log(">>> ETAPA 3: VALIDANDO PERSONAGENS NA CENA <<<");
        ValidarECorrigirPersonagensNaCena();
        Debug.Log("");
        
        // ETAPA 4: Verificar e configurar NavMesh
        Debug.Log(">>> ETAPA 4: CONFIGURANDO NAVMESH <<<");
        VerificarEConfigurarNavMesh();
        Debug.Log("");
        
        // ETAPA 5: Validar e corrigir animações
        Debug.Log(">>> ETAPA 5: VALIDANDO ANIMACOES <<<");
        ValidarECorrigirAnimacoes();
        Debug.Log("");
        
        // ETAPA 6: Forçar Animators a funcionarem
        Debug.Log(">>> ETAPA 6: FORCANDO ANIMATORS A FUNCIONAREM <<<");
        ForcarAnimatorsFuncionarem();
        Debug.Log("");
        
        // ETAPA 7: Salvar tudo
        Debug.Log(">>> ETAPA 7: SALVANDO ALTERACOES <<<");
        
        // Verificar se está em Play Mode antes de salvar
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("[AVISO] Script executado durante Play Mode. Algumas operacoes nao podem ser realizadas.");
            Debug.LogWarning("[AVISO] Saia do Play Mode e execute o script novamente para salvar a cena.");
        }
        else
        {
            // Salvar assets apenas se não estiver em Play Mode
            AssetDatabase.SaveAssets();
            
            // Marcar cena como modificada apenas se não estiver em Play Mode
            if (EditorSceneManager.GetActiveScene().IsValid())
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            
            Debug.Log("Assets salvos com sucesso");
        }
        Debug.Log("");
        
        Debug.Log("========================================");
        Debug.Log("=== CORRECAO COMPLETA FINALIZADA ===");
        Debug.Log($"Problemas encontrados: {problemasEncontrados.Count}");
        Debug.Log($"Correcoes aplicadas: {correcoesAplicadas.Count}");
        Debug.Log($"Finalizado em: {System.DateTime.Now}");
        Debug.Log("========================================");
        
        EditorUtility.DisplayDialog("Correção Completa",
            $"Correção completa finalizada!\n\n" +
            $"Problemas encontrados: {problemasEncontrados.Count}\n" +
            $"Correções aplicadas: {correcoesAplicadas.Count}\n\n" +
            $"Verifique o Console para detalhes completos.",
            "OK");
    }
    
    /// <summary>
    /// ETAPA 1: Remove scripts faltando (missing scripts) de todos os objetos na cena
    /// Usa o método nativo da Unity GameObjectUtility.RemoveMonoBehavioursWithMissingScript
    /// </summary>
    private void LimparScriptsFaltando()
    {
        Debug.Log("[SCRIPTS FALTANDO] Iniciando limpeza de scripts faltando...");
        
        int count = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"[SCRIPTS FALTANDO] Total de objetos na cena: {allObjects.Length}");
        
        foreach (GameObject obj in allObjects)
        {
            // Usar método nativo da Unity para remover scripts faltando
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
            if (removed > 0)
            {
                count += removed;
                Debug.Log($"[SCRIPTS FALTANDO] Removidos {removed} script(s) faltando de: {obj.name}");
                EditorUtility.SetDirty(obj);
            }
        }
        
        if (count > 0)
        {
            string correcao = $"Total de {count} script(s) faltando removidos";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[SCRIPTS FALTANDO] {correcao}");
        }
        else
        {
            Debug.Log("[SCRIPTS FALTANDO] Nenhum script faltando encontrado: OK");
        }
    }
    
    /// <summary>
    /// ETAPA 2: Valida e corrige Animator Controllers
    /// Verifica se existem, se têm estado Idle, se Idle tem animação, e se Idle é o estado padrão
    /// </summary>
    void ValidarECorrigirAnimatorControllers()
    {
        Debug.Log("[ANIMATOR CONTROLLERS] Iniciando validacao...");
        
        // PROTAGONISTA CONTROLLER
        Debug.Log("[PROTAGONISTA] Verificando existencia do arquivo...");
        string playerControllerPath = "Assets/Personagens/Protagonista/ProtagonistaController.controller";
        AnimatorController playerController = AssetDatabase.LoadAssetAtPath<AnimatorController>(playerControllerPath);
        
        if (playerController == null)
        {
            Debug.LogWarning("[PROTAGONISTA] Controller nao encontrado no caminho padrao, procurando...");
            // Tentar encontrar em outros caminhos
            string[] guids = AssetDatabase.FindAssets("ProtagonistaController t:AnimatorController");
            if (guids.Length > 0)
            {
                string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.Log($"[PROTAGONISTA] Controller encontrado em: {foundPath}");
                playerController = AssetDatabase.LoadAssetAtPath<AnimatorController>(foundPath);
            }
            
            if (playerController == null)
            {
                string msg = "ERRO: ProtagonistaController nao encontrado!";
                problemasEncontrados.Add(msg);
                Debug.LogError($"[PROTAGONISTA] {msg}");
                return;
            }
        }
        
        Debug.Log($"[PROTAGONISTA] Controller encontrado: {playerController.name}");
        ValidarECorrigirController(playerController, "Player");
        
        // ENEMY CONTROLLER
        Debug.Log("[ENEMY] Verificando existencia do arquivo...");
        string enemyControllerPath = "Assets/Personagens/CobraRasteira/CRController.controller";
        AnimatorController enemyController = AssetDatabase.LoadAssetAtPath<AnimatorController>(enemyControllerPath);
        
        if (enemyController == null)
        {
            Debug.LogWarning("[ENEMY] Controller nao encontrado no caminho padrao, procurando...");
            string[] guids = AssetDatabase.FindAssets("CRController t:AnimatorController");
            if (guids.Length > 0)
            {
                string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.Log($"[ENEMY] Controller encontrado em: {foundPath}");
                enemyController = AssetDatabase.LoadAssetAtPath<AnimatorController>(foundPath);
            }
            
            if (enemyController == null)
            {
                string msg = "ERRO: CRController nao encontrado!";
                problemasEncontrados.Add(msg);
                Debug.LogError($"[ENEMY] {msg}");
                return;
            }
        }
        
        Debug.Log($"[ENEMY] Controller encontrado: {enemyController.name}");
        ValidarECorrigirController(enemyController, "Enemy");
    }
    
    /// <summary>
    /// Valida e corrige um Animator Controller específico
    /// Verifica: estado Idle existe, Idle tem animação, Idle é estado padrão, transições para Idle
    /// </summary>
    /// <param name="controller">Controller a ser validado</param>
    /// <param name="tipo">Tipo do personagem (Player ou Enemy)</param>
    void ValidarECorrigirController(AnimatorController controller, string tipo)
    {
        Debug.Log($"[{tipo}] Validando controller: {controller.name}");
        
        if (controller.layers == null || controller.layers.Length == 0)
        {
            string msg = $"ERRO: Controller {controller.name} nao tem layers!";
            problemasEncontrados.Add(msg);
            Debug.LogError($"[{tipo}] {msg}");
            return;
        }
        
        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        if (stateMachine == null)
        {
            string msg = $"ERRO: StateMachine nao encontrado no {controller.name}!";
            problemasEncontrados.Add(msg);
            Debug.LogError($"[{tipo}] {msg}");
            return;
        }
        
        // 1. VERIFICAR E CRIAR ESTADO IDLE SE NECESSÁRIO
        Debug.Log($"[{tipo}] Procurando estado 'Idle'...");
        AnimatorState idleState = null;
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            if (state.state.name == "Idle")
            {
                idleState = state.state;
                Debug.Log($"[{tipo}] Estado 'Idle' encontrado: OK");
                break;
            }
        }
        
        if (idleState == null)
        {
            Debug.Log($"[{tipo}] Estado 'Idle' nao existe, criando...");
            idleState = stateMachine.AddState("Idle", new Vector3(0, 0, 0));
            idleState.name = "Idle";
            string correcao = $"Estado 'Idle' criado no {controller.name}";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[{tipo}] {correcao}");
        }
        
        // 2. VERIFICAR E CORRIGIR ANIMAÇÃO DO IDLE
        Debug.Log($"[{tipo}] Verificando animacao do estado 'Idle'...");
        bool animacaoCorreta = true;
        
        if (idleState.motion == null)
        {
            animacaoCorreta = false;
            Debug.LogWarning($"[{tipo}] Estado 'Idle' nao tem animacao!");
        }
        else
        {
            string nomeAnimacao = idleState.motion.name;
            Debug.Log($"[{tipo}] Estado 'Idle' tem animacao: {nomeAnimacao}");
            
            // Verificar se está usando animação errada (ataque em vez de idle)
            if (tipo == "Enemy")
            {
                if (nomeAnimacao.Contains("ChuteAlto") || nomeAnimacao.Contains("Pontera") || 
                    nomeAnimacao.Contains("Esquiva") || nomeAnimacao.Contains("Chute"))
                {
                    animacaoCorreta = false;
                    Debug.LogWarning($"[{tipo}] Estado 'Idle' esta usando animacao ERRADA: {nomeAnimacao}");
                }
            }
            else if (tipo == "Player")
            {
                if (nomeAnimacao.Contains("Chute") || nomeAnimacao.Contains("Bencao") || 
                    nomeAnimacao.Contains("Armada") || nomeAnimacao.Contains("Rasteira"))
                {
                    animacaoCorreta = false;
                    Debug.LogWarning($"[{tipo}] Estado 'Idle' esta usando animacao ERRADA: {nomeAnimacao}");
                }
            }
        }
        
        // Se animação está errada ou não existe, procurar e atribuir animação correta
        if (!animacaoCorreta)
        {
            Debug.Log($"[{tipo}] Procurando animacao correta para Idle...");
            AnimationClip clipCorreto = null;
            
            if (tipo == "Player")
            {
                // Procurar animação Capoeira para Player
                string[] paths = {
                    "Assets/Animations/Protagonista/Protagonista@Capoeira.fbx",
                    "Assets/Animations/protagonista@Capoeira.fbx"
                };
                
                foreach (string path in paths)
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (Object asset in assets)
                    {
                        if (asset is AnimationClip && asset.name.Contains("Capoeira"))
                        {
                            clipCorreto = asset as AnimationClip;
                            break;
                        }
                    }
                    if (clipCorreto != null) break;
                }
            }
            else if (tipo == "Enemy")
            {
                // Procurar animação Ginga para Enemy
                string[] paths = {
                    "Assets/Animations/Vilão 1 - Cobra rasteira/Vilao@Ginga.fbx",
                    "Assets/Animations/Vilao 1 - Cobra rasteira/Vilao@Ginga.fbx"
                };
                
                foreach (string path in paths)
                {
                    Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                    foreach (Object asset in assets)
                    {
                        if (asset is AnimationClip && asset.name.Contains("Ginga"))
                        {
                            clipCorreto = asset as AnimationClip;
                            break;
                        }
                    }
                    if (clipCorreto != null) break;
                }
                
                // Se não encontrar Ginga, tentar Esquiva
                if (clipCorreto == null)
                {
                    string[] pathsEsquiva = {
                        "Assets/Animations/Vilão 1 - Cobra rasteira/Vilao@Esquiva.fbx"
                    };
                    
                    foreach (string path in pathsEsquiva)
                    {
                        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                        foreach (Object asset in assets)
                        {
                            if (asset is AnimationClip)
                            {
                                clipCorreto = asset as AnimationClip;
                                break;
                            }
                        }
                        if (clipCorreto != null) break;
                    }
                }
            }
            
            if (clipCorreto != null)
            {
                idleState.motion = clipCorreto;
                string correcao = $"Animacao '{clipCorreto.name}' atribuida ao estado 'Idle' no {controller.name}";
                correcoesAplicadas.Add(correcao);
                Debug.Log($"[{tipo}] {correcao}");
            }
            else
            {
                string msg = $"ERRO: Nao foi possivel encontrar animacao apropriada para Idle no {controller.name}";
                problemasEncontrados.Add(msg);
                Debug.LogError($"[{tipo}] {msg}");
            }
        }
        
        // 3. DEFINIR IDLE COMO ESTADO PADRÃO
        Debug.Log($"[{tipo}] Verificando estado padrao...");
        if (stateMachine.defaultState != idleState)
        {
            string estadoAtual = stateMachine.defaultState != null ? stateMachine.defaultState.name : "NULL";
            Debug.LogWarning($"[{tipo}] Estado padrao nao e 'Idle' (e '{estadoAtual}'), corrigindo...");
            stateMachine.defaultState = idleState;
            string correcao = $"'Idle' definido como estado padrao no {controller.name}";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[{tipo}] {correcao}");
        }
        else
        {
            Debug.Log($"[{tipo}] 'Idle' ja e o estado padrao: OK");
        }
        
        // 4. GARANTIR TRANSIÇÕES PARA IDLE
        Debug.Log($"[{tipo}] Verificando transicoes para Idle...");
        int transicoesCriadas = 0;
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            if (state.state != idleState)
            {
                bool temTransicao = false;
                foreach (AnimatorStateTransition trans in state.state.transitions)
                {
                    if (trans.destinationState == idleState)
                    {
                        temTransicao = true;
                        break;
                    }
                }
                
                if (!temTransicao)
                {
                    AnimatorStateTransition trans = state.state.AddTransition(idleState);
                    trans.hasExitTime = true;
                    trans.exitTime = 0.9f;
                    trans.duration = 0.25f;
                    transicoesCriadas++;
                }
            }
        }
        
        if (transicoesCriadas > 0)
        {
            string correcao = $"{transicoesCriadas} transicoes criadas para 'Idle' no {controller.name}";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[{tipo}] {correcao}");
        }
        
        EditorUtility.SetDirty(controller);
    }
    
    /// <summary>
    /// ETAPA 3: Valida e corrige personagens na cena
    /// Verifica: Animator existe, Controller está atribuído, Avatar está configurado, Animator está habilitado
    /// </summary>
    void ValidarECorrigirPersonagensNaCena()
    {
        Debug.Log("[PERSONAGENS] Iniciando validacao de personagens na cena...");
        
        // PLAYER
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        if (player == null)
        {
            player = GameObject.Find("protagonista");
        }
        if (player == null)
        {
            player = GameObject.Find("Protagonista");
        }
        
        if (player != null)
        {
            Debug.Log($"[PLAYER] Player encontrado: {player.name}");
            ValidarECorrigirPersonagem(player, "Player", "ProtagonistaController");
        }
        else
        {
            string msg = "ERRO: Player nao encontrado na cena!";
            problemasEncontrados.Add(msg);
            Debug.LogError($"[PLAYER] {msg}");
        }
        
        // ENEMY
        GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
        if (enemy == null)
        {
            enemy = GameObject.Find("Enemy");
        }
        if (enemy == null)
        {
            enemy = GameObject.Find("CobraRasteira");
        }
        
        if (enemy != null)
        {
            Debug.Log($"[ENEMY] Enemy encontrado: {enemy.name}");
            ValidarECorrigirPersonagem(enemy, "Enemy", "CRController");
        }
        else
        {
            string msg = "ERRO: Enemy nao encontrado na cena!";
            problemasEncontrados.Add(msg);
            Debug.LogError($"[ENEMY] {msg}");
        }
    }
    
    /// <summary>
    /// Valida e corrige um personagem específico na cena
    /// Verifica e corrige: Animator, Controller, Avatar, configurações do Animator
    /// </summary>
    /// <param name="personagem">GameObject do personagem</param>
    /// <param name="tipo">Tipo do personagem (Player ou Enemy)</param>
    /// <param name="controllerName">Nome do Controller esperado</param>
    void ValidarECorrigirPersonagem(GameObject personagem, string tipo, string controllerName)
    {
        Debug.Log($"[{tipo}] Validando personagem: {personagem.name}");
        
        // 1. VERIFICAR E ADICIONAR ANIMATOR
        Animator anim = personagem.GetComponent<Animator>();
        if (anim == null)
        {
            Debug.Log($"[{tipo}] Animator nao encontrado, adicionando...");
            anim = personagem.AddComponent<Animator>();
            string correcao = $"Animator adicionado ao {tipo}";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[{tipo}] {correcao}");
        }
        
        // 2. VERIFICAR E ATRIBUIR CONTROLLER
        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"[{tipo}] Controller nao atribuido, procurando...");
            string controllerPath = $"Assets/Personagens/{(tipo == "Player" ? "Protagonista" : "CobraRasteira")}/{controllerName}.controller";
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(controllerPath);
            
            if (controller == null)
            {
                string[] guids = AssetDatabase.FindAssets($"{controllerName} t:AnimatorController");
                if (guids.Length > 0)
                {
                    string foundPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(foundPath);
                }
            }
            
            if (controller != null)
            {
                anim.runtimeAnimatorController = controller;
                string correcao = $"Controller '{controllerName}' atribuido ao {tipo}";
                correcoesAplicadas.Add(correcao);
                Debug.Log($"[{tipo}] {correcao}");
            }
            else
            {
                string msg = $"ERRO: Controller '{controllerName}' nao encontrado para {tipo}!";
                problemasEncontrados.Add(msg);
                Debug.LogError($"[{tipo}] {msg}");
            }
        }
        else
        {
            Debug.Log($"[{tipo}] Controller encontrado: {anim.runtimeAnimatorController.name}");
        }
        
        // 3. VERIFICAR AVATAR (importante para animações Humanoid)
        if (anim.avatar == null)
        {
            string msg = $"{tipo} nao tem Avatar atribuido!";
            problemasEncontrados.Add(msg);
            Debug.LogWarning($"[{tipo}] {msg} - Isso pode causar T-pose!");
        }
        else
        {
            Debug.Log($"[{tipo}] Avatar encontrado: {anim.avatar.name} (Valido: {anim.avatar.isValid}, Human: {anim.avatar.isHuman})");
        }
        
        // 4. CONFIGURAR ANIMATOR
        if (!anim.enabled)
        {
            anim.enabled = true;
            string correcao = $"Animator do {tipo} estava desabilitado, habilitado agora";
            correcoesAplicadas.Add(correcao);
            Debug.Log($"[{tipo}] {correcao}");
        }
        
        anim.updateMode = AnimatorUpdateMode.Normal;
        anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        Debug.Log($"[{tipo}] Animator configurado: UpdateMode=Normal, CullingMode=AlwaysAnimate");
        
        // 5. FORÇAR IDLE (tenta evitar T-pose)
        if (anim.runtimeAnimatorController != null)
        {
            anim.Rebind();
            try
            {
                anim.Play("Idle", 0, 0f);
                anim.Update(0.1f);
                Debug.Log($"[{tipo}] Idle forcado com sucesso");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[{tipo}] Erro ao forcar Idle: {e.Message}");
            }
        }
        
        EditorUtility.SetDirty(personagem);
    }
    
    /// <summary>
    /// ETAPA 4: Verifica e configura NavMesh
    /// Verifica se NavMesh existe, posiciona personagens no NavMesh, configura NavMeshAgent
    /// </summary>
    private void VerificarEConfigurarNavMesh()
    {
        Debug.Log("[NAVMESH] Verificando NavMesh...");
        
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        
        if (navMeshData.vertices.Length == 0)
        {
            string msg = "NavMesh nao encontrado! E necessario gerar o NavMesh.";
            problemasEncontrados.Add(msg);
            Debug.LogWarning($"[NAVMESH] {msg}");
            Debug.LogWarning("[NAVMESH] Para gerar: Window > AI > Navigation > Bake");
        }
        else
        {
            Debug.Log($"[NAVMESH] NavMesh encontrado com {navMeshData.vertices.Length} vertices: OK");
            
            // Posicionar Player no NavMesh
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Vector3 playerPos = new Vector3(-2.5f, 0f, 0f);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(playerPos, out hit, 20f, NavMesh.AllAreas))
                {
                    player.transform.position = hit.position;
                    Debug.Log($"[NAVMESH] Player posicionado no NavMesh: {hit.position}");
                }
            }
            
            // Posicionar Enemy no NavMesh
            GameObject enemy = GameObject.FindGameObjectWithTag("Enemy");
            if (enemy != null)
            {
                Vector3 enemyPos = new Vector3(2.5f, 0f, 0f);
                NavMeshHit hit;
                bool found = false;
                
                if (NavMesh.SamplePosition(enemyPos, out hit, 20f, NavMesh.AllAreas))
                {
                    enemy.transform.position = hit.position;
                    found = true;
                }
                else
                {
                    // Buscar em área maior
                    for (float radius = 1f; radius <= 50f && !found; radius += 2f)
                    {
                        for (int angle = 0; angle < 360; angle += 15)
                        {
                            float rad = angle * Mathf.Deg2Rad;
                            Vector3 testPos = enemyPos + new Vector3(Mathf.Cos(rad) * radius, 0, Mathf.Sin(rad) * radius);
                            
                            if (NavMesh.SamplePosition(testPos, out hit, 5f, NavMesh.AllAreas))
                            {
                                enemy.transform.position = hit.position;
                                found = true;
                                break;
                            }
                        }
                    }
                }
                
                // Configurar NavMeshAgent
                NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
                if (agent != null && found)
                {
                    agent.enabled = true;
                    agent.Warp(hit.position);
                    Debug.Log($"[NAVMESH] NavMeshAgent configurado para Enemy");
                }
            }
        }
    }
    
    /// <summary>
    /// ETAPA 5: Valida se as animações principais existem no projeto
    /// </summary>
    void ValidarECorrigirAnimacoes()
    {
        Debug.Log("[ANIMACOES] Validando existencia de animacoes...");
        
        string[] animacoesPlayer = {
            "Assets/Animations/Protagonista/Protagonista@Capoeira.fbx"
        };
        
        string[] animacoesEnemy = {
            "Assets/Animations/Vilão 1 - Cobra rasteira/Vilao@Ginga.fbx"
        };
        
        foreach (string path in animacoesPlayer)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            bool encontrouClip = false;
            foreach (Object asset in assets)
            {
                if (asset is AnimationClip)
                {
                    encontrouClip = true;
                    Debug.Log($"[ANIMACOES] Player: {asset.name} encontrado");
                    break;
                }
            }
            if (!encontrouClip)
            {
                Debug.LogWarning($"[ANIMACOES] Nenhum AnimationClip encontrado em: {path}");
            }
        }
        
        foreach (string path in animacoesEnemy)
        {
            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
            bool encontrouClip = false;
            foreach (Object asset in assets)
            {
                if (asset is AnimationClip)
                {
                    encontrouClip = true;
                    Debug.Log($"[ANIMACOES] Enemy: {asset.name} encontrado");
                    break;
                }
            }
            if (!encontrouClip)
            {
                Debug.LogWarning($"[ANIMACOES] Nenhum AnimationClip encontrado em: {path}");
            }
        }
    }
    
    /// <summary>
    /// ETAPA 6: Força todos os Animators na cena a funcionarem corretamente
    /// Habilita Animators, configura Update/Culling Mode, executa Rebind e força Idle
    /// </summary>
    void ForcarAnimatorsFuncionarem()
    {
        Debug.Log("[FORCAR ANIMATORS] Iniciando processo...");
        
        Animator[] animators = Object.FindObjectsOfType<Animator>();
        Debug.Log($"[FORCAR ANIMATORS] Total de Animators encontrados: {animators.Length}");
        
        int processados = 0;
        foreach (Animator anim in animators)
        {
            if (anim == null || !anim.gameObject.activeInHierarchy) continue;
            
            processados++;
            Debug.Log($"[FORCAR ANIMATORS] Processando: {anim.gameObject.name}");
            
            // Habilitar se estiver desabilitado
            if (!anim.enabled)
            {
                anim.enabled = true;
                Debug.Log($"[FORCAR ANIMATORS] Animator habilitado");
            }
            
            // Configurar Update e Culling Mode
            anim.updateMode = AnimatorUpdateMode.Normal;
            anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            
            // Se tem Controller, forçar Rebind e Idle
            if (anim.runtimeAnimatorController != null)
            {
                anim.Rebind();
                try
                {
                    anim.Play("Idle", 0, 0f);
                    anim.Update(0.1f);
                    Debug.Log($"[FORCAR ANIMATORS] Idle forcado com sucesso");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[FORCAR ANIMATORS] Erro ao forcar Idle: {e.Message}");
                }
            }
            
            EditorUtility.SetDirty(anim.gameObject);
        }
        
        Debug.Log($"[FORCAR ANIMATORS] Processamento concluido: {processados} Animators processados");
    }
}
