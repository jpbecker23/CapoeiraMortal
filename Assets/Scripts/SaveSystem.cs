using UnityEngine;

/// <summary>
/// Sistema de salvamento e carregamento de progresso do jogo.
/// </summary>
public static class SaveSystem
{
    private const string LEVEL_KEY = "CurrentLevel";
    private const string HIGHEST_LEVEL_KEY = "HighestLevel";
    private const string TOTAL_WINS_KEY = "TotalWins";
    private const string TOTAL_LOSSES_KEY = "TotalLosses";
    
    /// <summary>
    /// Salva o nível atual do jogador.
    /// </summary>
    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(LEVEL_KEY, level);
        
        // Salvar maior nível alcançado
        int highestLevel = GetHighestLevel();
        if (level > highestLevel)
        {
            PlayerPrefs.SetInt(HIGHEST_LEVEL_KEY, level);
        }
        
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Carrega o nível salvo do jogador.
    /// </summary>
    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt(LEVEL_KEY, 1);
    }
    
    /// <summary>
    /// Obtém o maior nível alcançado.
    /// </summary>
    public static int GetHighestLevel()
    {
        return PlayerPrefs.GetInt(HIGHEST_LEVEL_KEY, 1);
    }
    
    /// <summary>
    /// Incrementa contador de vitórias.
    /// </summary>
    public static void AddWin()
    {
        int wins = GetTotalWins();
        PlayerPrefs.SetInt(TOTAL_WINS_KEY, wins + 1);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Incrementa contador de derrotas.
    /// </summary>
    public static void AddLoss()
    {
        int losses = GetTotalLosses();
        PlayerPrefs.SetInt(TOTAL_LOSSES_KEY, losses + 1);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Obtém total de vitórias.
    /// </summary>
    public static int GetTotalWins()
    {
        return PlayerPrefs.GetInt(TOTAL_WINS_KEY, 0);
    }
    
    /// <summary>
    /// Obtém total de derrotas.
    /// </summary>
    public static int GetTotalLosses()
    {
        return PlayerPrefs.GetInt(TOTAL_LOSSES_KEY, 0);
    }
    
    /// <summary>
    /// Reseta todo o progresso salvo.
    /// </summary>
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.DeleteKey(HIGHEST_LEVEL_KEY);
        PlayerPrefs.DeleteKey(TOTAL_WINS_KEY);
        PlayerPrefs.DeleteKey(TOTAL_LOSSES_KEY);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Salva todas as configurações.
    /// </summary>
    public static void SaveAll()
    {
        PlayerPrefs.Save();
    }
}

