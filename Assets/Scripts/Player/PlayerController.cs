    using System;
using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    const float offsetY = 0.3f;

    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask grassLayer;
    public LayerMask winninglayer;

    public event Action OnEncountered;

    public bool IsMoving;
    private Vector2 input;

    private Animator animator;
    [SerializeField]private XeomonParty xeomonParty;
    [SerializeField] FloatSO starterNumber;
    [SerializeField] Xeomon[] starters;
    [SerializeField] BoolSO isGameOver;
    private void Awake()
    {
        xeomonParty.Reset();
        animator = GetComponent<Animator>();
        if (xeomonParty.Xeomons.Count == 0)
        {
            if (starterNumber.value == 1)
            {
                xeomonParty.AddXeomon(starters[0]);
            }
            else if (starterNumber.value == 2)
            {
                xeomonParty.AddXeomon(starters[1]);
            }
            else if (starterNumber.value == 3)
            {
                xeomonParty.AddXeomon(starters[2]);
            }
        }
    }

    public void HandleUpdate()
    {
        if (!IsMoving)
        {
            input.x = 0;
            input.y = 0;

            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.dKey.isPressed) input.x = 1;
                else if (keyboard.aKey.isPressed) input.x = -1;

                if (input.x == 0)
                {
                    if (keyboard.wKey.isPressed) input.y = 1;
                    else if (keyboard.sKey.isPressed) input.y = -1;
                }
            }

            if (input != Vector2.zero)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos));
            }
        }

        animator.SetBool("IsMoving", IsMoving);

        if(xeomonParty.Xeomons.Count == 0)
        {
            isGameOver.value = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Endgame");
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        IsMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        IsMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayer) != null)
            return false;
        return true;
    }

    private void CheckForEncounters()
    {
        //if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, grassLayer) != null)
        //{
        //    if (UnityEngine.Random.Range(1, 101) <= 10)
        //    {
        //        animator.SetBool("IsMoving", false);
        //        OnEncountered();
        //    }
        //}

        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, offsetY), 0.2f, winninglayer) != null)
        {
            isGameOver.value = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Endgame");
        }
    }
}
