using System.Collections;
using UnityEngine;
public enum EffectType
{
    Start,
    Idle,
    End
}
public class EffectController : MonoBehaviour
{
    [Header("Animation Settings")]
    public Sprite[] frames;          // кадры анимации
    public float frameRate = 0.05f;  // время между кадрами
    public SpriteRenderer sr;
    public EffectType type = EffectType.Idle;
    public EffectController nextAnim;
    [Header("Movement Settings")]
    public Transform target;         // цель
    public float moveSpeed = 5f;

    public void Play(Transform target = null, float moveSpeed = 5f)
    {
        if (target != null || type == EffectType.Start)
            StartCoroutine(PlayEffect());
        else
        {
            this.target = target;
            this.moveSpeed = moveSpeed;
            StartCoroutine(PlayAndMove());
        }

    }

    IEnumerator PlayEffect()
    {

        for (int i = 0; i < frames.Length; i++)
        {
            sr.sprite = frames[i];
            yield return new WaitForSeconds(frameRate);
        }
        if (nextAnim != null)
        {
            EffectController nA = Instantiate(nextAnim.gameObject, transform.position, Quaternion.identity).GetComponent<EffectController>();
            nA.Play();
        }

        Destroy(gameObject);
    }
    private IEnumerator PlayAndMove()
    {
        int frameIndex = 0;
        float timer = 0f;

        while (true)
        {
            // ======== Move ========
            if (target != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                        target.position,
                            moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, target.position) <= 0.1f)
                {
                    break;
                }
            }
            else
                break;

            // ======== Anim ========
            timer += Time.deltaTime;
            if (timer >= frameRate)
            {
                timer = 0f;
                sr.sprite = frames[frameIndex];
                frameIndex++;

                if (frameIndex >= frames.Length)
                {
                    frameIndex = 0;
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
