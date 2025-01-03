using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatusTartaruga : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI nickname;
    [SerializeField] Slider healCooldownSlider;
    [SerializeField] GameObject confetes;

    public float vidaAtual;
    public bool podeCurar;
    float plasticosComidos;

    public static StatusTartaruga instance;

    void Start()
    {
        string _savedNickname = PlayerPrefs.GetString("NicknameFilhote", "Casquinha");

        if (!string.IsNullOrEmpty(_savedNickname))
            nickname.text = _savedNickname;
        else
        {
            nickname.text = "Casquinha";
            Debug.LogWarning("Nickname n�o foi definido. Usando nome padr�o.");
        }

        instance = this;
        podeCurar = true;
        vidaAtual = 1;
        plasticosComidos = 0;
        AtualizarBarra(vidaAtual);
        confetes.SetActive(false);
    }

    private void Update()
    {
        if (vidaAtual <= 0)
            GameManager.instance.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CuraBloqueada()
    {
        podeCurar = false;
        yield return new WaitForSeconds(30f);
        plasticosComidos = 0;
        healCooldownSlider.value = plasticosComidos;
        podeCurar = true;
    }

    void AtualizarBarra(float vida) => healthBar.fillAmount = vida;

    public void ReceberDano(float dano)
    {
        vidaAtual -= dano;
        AtualizarBarra(vidaAtual);
    }

    public void RecuperarVida(float cura)
    {
        vidaAtual += cura;
        AtualizarBarra(vidaAtual);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BlockCura"))
        {
            Destroy(other.gameObject);
            plasticosComidos += 0.35f;
            healCooldownSlider.value = plasticosComidos;

            if (healCooldownSlider.value >= 1)
                StartCoroutine(CuraBloqueada());
        }

        if (other.gameObject.CompareTag("DestinoMar"))
        {
            confetes.SetActive(true);
            Invoke(nameof(Concluir), 7f);
        }
    }

    void Concluir()
    {
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;
        int proximaCena = GameManager.proximaEtapa;

        Debug.Log($"Cena Atual: {cenaAtual}, Pr�xima Cena: {proximaCena}");

        if (proximaCena <= cenaAtual)
        {
            proximaCena = cenaAtual + 1;
            GameManager.proximaEtapa = proximaCena;
        }

        if (proximaCena < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(proximaCena);
        else
            Debug.LogError("Proxima cena est� fora do range das cenas configuradas no Build Settings.");
    }
}