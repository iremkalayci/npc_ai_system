using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroManager : MonoBehaviour
{
    public Text introText;
    public GameObject player;
    public float displayDuration = 6f; // toplam süre (Enter'a basmazsa)

    private bool skipped = false;

    void Start()
    {
        // Oyun başında oyuncuyu kapat
        if (player != null)
            player.GetComponent<CharacterController>().enabled = false;

        StartCoroutine(ShowIntroText());
    }

    IEnumerator ShowIntroText()
    {
        introText.text =
            "Yıl 2089...\nAy üssü saldırı altında.\nTüm iletişim kesildi.\n\n" +
            "Görev: Tüm uzaylıları yok et ve iletişim noktasına ulaşarak kurtul!";

        Color c = introText.color;
        c.a = 1;
        introText.color = c;

        float elapsed = 0f;

        while (elapsed < displayDuration && !skipped)
        {
            elapsed += Time.deltaTime;

            // Kullanıcı Enter'a bastıysa çık
            if (Input.GetKeyDown(KeyCode.Return))
            {
                skipped = true;
                break;
            }

            yield return null;
        }

        // Yavaşça kaybolma efekti (Enter'a bassan da animasyonlu gider)
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            c.a = Mathf.Lerp(1, 0, t / 1f);
            introText.color = c;
            yield return null;
        }

        introText.gameObject.SetActive(false);

        // Oyuncuyu aktif et
        if (player != null)
            player.GetComponent<CharacterController>().enabled = true;
    }
}
