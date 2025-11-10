# 🔊 GUIA COMPLETO DE ÁUDIO - CAPOEIRA MORTAL

## 📁 ESTRUTURA DA PASTA _Audio

```
Assets/_Audio/
├── SFX/                    # Efeitos Sonoros
│   ├── Attacks/           # Sons de ataques
│   ├── Hits/              # Sons de impacto
│   ├── Dodge/             # Sons de esquiva
│   ├── Death/             # Sons de morte
│   └── UI/                # Sons de interface
├── BGM/                    # Música de Fundo
│   ├── Menu/              # Música do menu
│   └── Gameplay/          # Música durante o jogo
└── AudioMixer/            # Mixer de áudio
    └── MainMixer.mixer    # Mixer principal
```

---

## 🎵 FORMATOS DE ÁUDIO RECOMENDADOS

### **Formato Principal: OGG Vorbis**
- ✅ **Melhor compressão** sem perda perceptível de qualidade
- ✅ **Menor tamanho de arquivo**
- ✅ **Suportado nativamente pelo Unity**
- ✅ **Ideal para música de fundo**

### **Formato Alternativo: WAV**
- ✅ **Sem compressão** (qualidade máxima)
- ✅ **Ideal para SFX curtos** (ataques, hits)
- ⚠️ **Arquivos maiores**
- ✅ **Sem delay de carregamento**

### **Formato Não Recomendado: MP3**
- ❌ **Problemas de licenciamento** em alguns casos
- ❌ **Compressão com perda**
- ❌ **Não recomendado pelo Unity**

---

## 📊 ESPECIFICAÇÕES TÉCNICAS

### **Música de Fundo (BGM)**
- **Formato:** OGG Vorbis
- **Taxa de Amostragem:** 44.1 kHz ou 48 kHz
- **Bitrate:** 128-192 kbps
- **Canais:** Estéreo (2.0)
- **Duração:** Loop contínuo (sem fade)
- **Tamanho Recomendado:** < 5 MB por música

### **Efeitos Sonoros (SFX)**
- **Formato:** WAV ou OGG Vorbis
- **Taxa de Amostragem:** 44.1 kHz
- **Bitrate:** 128 kbps (OGG) ou PCM (WAV)
- **Canais:** Mono (1.0) para SFX simples, Estéreo para ambientes
- **Duração:** Curta (0.1s - 2s)
- **Tamanho Recomendado:** < 500 KB por som

---

## 🎮 COMO ADICIONAR ÁUDIOS NO UNITY

### **1. Criar Estrutura de Pastas**

1. No Unity, vá em **Assets**
2. Crie a pasta `_Audio` (se não existir)
3. Dentro de `_Audio`, crie:
   - `SFX/`
   - `BGM/`
   - `AudioMixer/`

4. Dentro de `SFX/`, crie:
   - `Attacks/`
   - `Hits/`
   - `Dodge/`
   - `Death/`
   - `UI/`

5. Dentro de `BGM/`, crie:
   - `Menu/`
   - `Gameplay/`

### **2. Importar Arquivos de Áudio**

1. **Arraste os arquivos** para as pastas correspondentes
2. **Selecione o arquivo** no Project
3. No **Inspector**, configure:

#### **Para SFX (Efeitos Sonoros):**
```
Audio Type: SFX
Load Type: Decompress On Load (para sons curtos)
           ou Compressed In Memory (para sons longos)
Compression Format: Vorbis (OGG) ou PCM (WAV)
Quality: 70-90 (para OGG)
Sample Rate: 44100 Hz
```

#### **Para BGM (Música):**
```
Audio Type: Music
Load Type: Streaming (recomendado para música longa)
Compression Format: Vorbis
Quality: 80-100
Sample Rate: 44100 Hz
```

### **3. Configurar AudioManager**

1. **Encontre o GameObject "AudioManager"** na cena (ou crie um)
2. **No Inspector**, expanda os arrays:
   - `Attack Sounds`
   - `Hit Sounds`
   - `Dodge Sounds`
   - `Death Sounds`
   - `UI Sounds`
   - `Background Music`

3. **Arraste os arquivos de áudio** para os arrays correspondentes:
   - **Attack Sounds**: Sons de ataques (Bencao, Armada, etc.)
   - **Hit Sounds**: Sons de impacto quando acerta
   - **Dodge Sounds**: Sons de esquiva
   - **Death Sounds**: Sons de morte
   - **UI Sounds**: Sons de botões, menus
   - **Background Music**: Músicas de fundo

### **4. Criar Audio Mixer (Opcional mas Recomendado)**

1. **Assets > Create > Audio > Audio Mixer**
2. Renomeie para `MainMixer`
3. **Abra o Audio Mixer** (Window > Audio > Audio Mixer)
4. **Crie Groups:**
   - Master
   - SFX
   - BGM

5. **Configure os Groups:**
   - **Master**: Volume geral
   - **SFX**: Efeitos sonoros (pode adicionar compressão)
   - **BGM**: Música (pode adicionar reverb)

6. **No AudioManager**, arraste os Groups para:
   - `Master Mixer Group`
   - `SFX Mixer Group`
   - `BGM Mixer Group`

---

## 📝 EXEMPLOS DE NOMENCLATURA

### **Sons de Ataque:**
```
attack_bencao_01.ogg
attack_armada_01.ogg
attack_chapa_01.ogg
attack_rasteira_01.ogg
attack_couro_01.ogg
```

### **Sons de Hit:**
```
hit_punch_01.wav
hit_kick_01.wav
hit_heavy_01.wav
```

### **Sons de Esquiva:**
```
dodge_quick_01.wav
dodge_roll_01.wav
```

### **Sons de Morte:**
```
death_player_01.wav
death_enemy_01.wav
```

### **Sons de UI:**
```
ui_button_click.wav
ui_button_hover.wav
ui_level_complete.wav
ui_game_over.wav
```

### **Música:**
```
bgm_menu_theme.ogg
bgm_gameplay_01.ogg
bgm_gameplay_02.ogg
```

---

## 🎚️ CONFIGURAÇÕES DE VOLUME RECOMENDADAS

### **Volumes Padrão:**
- **Master**: 100%
- **SFX**: 80-90%
- **BGM**: 60-70%

### **Por que BGM mais baixo?**
- Música de fundo não deve competir com SFX
- SFX precisam ser claros para feedback do jogador
- BGM é ambiente, não informação crítica

---

## 🔧 CONFIGURAÇÃO NO INSPECTOR

### **AudioManager Component:**

```
[Header("Audio Mixer")]
Master Mixer Group: [Arraste MainMixer > Master]
SFX Mixer Group: [Arraste MainMixer > SFX]
BGM Mixer Group: [Arraste MainMixer > BGM]

[Header("Configurações de Volume")]
Master Volume: 1.0
SFX Volume: 0.85
BGM Volume: 0.65

[Header("Clips de Áudio")]
Attack Sounds: [Array com sons de ataque]
Hit Sounds: [Array com sons de hit]
Dodge Sounds: [Array com sons de esquiva]
Death Sounds: [Array com sons de morte]
UI Sounds: [Array com sons de UI]
Background Music: [Array com músicas]
```

---

## ✅ CHECKLIST DE CONFIGURAÇÃO

- [ ] Estrutura de pastas criada
- [ ] Arquivos de áudio importados
- [ ] Configurações de importação corretas
- [ ] AudioManager configurado na cena
- [ ] Arrays de áudio preenchidos
- [ ] Audio Mixer criado e configurado
- [ ] Groups do Mixer conectados ao AudioManager
- [ ] Volumes ajustados
- [ ] Testado no jogo

---

## 🎯 DICAS IMPORTANTES

1. **Use OGG para música** (menor tamanho)
2. **Use WAV para SFX curtos** (sem delay)
3. **Evite MP3** (problemas de licença)
4. **Mantenha SFX em Mono** (economiza espaço)
5. **Use Streaming para BGM longa** (economiza RAM)
6. **Compressão Vorbis 70-90** (boa qualidade/tamanho)
7. **Sample Rate 44.1 kHz** (padrão, suficiente)
8. **Teste volumes** antes de finalizar

---

## 🐛 TROUBLESHOOTING

### **Problema: Áudio não toca**
- Verifique se AudioManager está na cena
- Verifique se os arrays estão preenchidos
- Verifique se os volumes não estão em 0
- Verifique se o AudioSource está configurado

### **Problema: Áudio cortado**
- Aumente o tamanho do buffer no AudioSource
- Mude Load Type para "Decompress On Load"

### **Problema: Áudio com delay**
- Use WAV em vez de OGG para SFX
- Mude Load Type para "Decompress On Load"
- Reduza a compressão

### **Problema: Música não faz loop**
- Verifique se "Loop" está marcado no AudioSource
- Verifique se a música tem fade no final (deve remover)

---

## 📚 RECURSOS ÚTEIS

- **Freesound.org**: Sons gratuitos
- **Zapsplat.com**: Biblioteca de SFX
- **Incompetech.com**: Músicas gratuitas (Kevin MacLeod)
- **Audacity**: Editor de áudio gratuito
- **Unity Audio Documentation**: https://docs.unity3d.com/Manual/class-AudioClip.html

---

**Última atualização:** 2024  
**Versão:** 1.0

