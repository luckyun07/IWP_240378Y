using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered;

    public bool IsMoving;
    private Vector2 input;

    private Animator animator;
    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("IsMoving", false);
                OnEncountered();
            }
        }
    }
}
