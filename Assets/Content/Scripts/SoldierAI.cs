using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SoldierAI : MonoBehaviour
{

    public ArmyCollection enemy;
    public enum State
    {
        Idle,
        Wandering,
        Attacking,
        Fleeing
    };

    public bool gettingClose = false;
    public State state = State.Idle;

    public float aggressiveness = 0.5f;
    public float cowardice = 0.5f;

    public float wanderRangeMin = 5.0f;
    public float wanderRangeMax = 20.0f;
    public float idleTimeMin = 2.0f;
    public float idleTimeMax = 5.0f;

    NavMeshAgent m_agent;
    SoldierController m_controller;
    Animator m_animator;

    Transform target;
    SoldierController targetController;

    bool m_attacking = false;
    public int offset;
    int frame = 0;

    // Use this for initialization
    void Start()
    {
        frame = offset;
        m_agent = gameObject.GetComponent<NavMeshAgent>();
        m_controller = gameObject.GetComponent<SoldierController>();
        m_animator = gameObject.GetComponent<Animator>();
        PickNewState();
        if (target != null)
        {
            transform.LookAt(target);
        }
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_controller.alive)
        {
            return;
        }

        m_animator.SetFloat("Speed", m_agent.velocity.magnitude);
        if(frame % 3 == 0)
        {
            if (state == State.Attacking && !enemy.Contains(targetController))
            {
                PickNewState();
            }
            else
            {
                Action();
            }

        }
        frame++;
    }

    private void Action()
    {
        switch (state)
        {
            case State.Wandering:
                 //Debug.DrawLine(gameObject.transform.position, m_agent.destination, Color.white);

                if (m_agent.remainingDistance < 2)
                {
                    PickNewState();
                }
                break;

            case State.Fleeing:
                if (target != null && targetController.alive)
                {
                    m_animator.SetBool("Fleeing", true);
                    //Debug.DrawLine(gameObject.transform.position, m_agent.destination, Color.black);

                    if (m_agent.remainingDistance < 4)
                    {
                        // Reached the fleeing destination
                        PickNewState();
                    }
                }
                else
                {
                    PickNewState();
                }
                break;

            case State.Attacking:
                if (target != null && targetController.alive)
                {
                    if (!m_attacking)
                    {
                        m_agent.destination = targetController.transform.position;
                    }
                    //Debug.DrawLine(gameObject.transform.position, target.position, m_controller.teamNumber == 0 ? Color.green : Color.red);
                    if (!m_agent.pathPending)
                    {
                        if (!gettingClose && m_agent.remainingDistance < 5)
                        {
                            gettingClose = true;
                            targetController.GetComponent<SoldierAI>().EnemyApproaching(transform);
                        }
                        else if (m_agent.remainingDistance < m_agent.stoppingDistance)
                        {
                            if (!m_attacking)
                            {
                                m_attacking = true;
                                m_animator.SetTrigger("Attack");
                            }

                            // Rotate towards the enemy
                            Vector3 targetDiff = targetController.transform.position - transform.position;
                            if (targetDiff != Vector3.zero)
                            {
                                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDiff),
                                    2 * Time.fixedDeltaTime);
                            }
                        }
                    }
                }
                else
                {
                    PickNewState();
                }
                break;

            default:
                break;
        }
    }

    public void EnemyApproaching(Transform enemy)
    {
        if (Random.Range(0.0f, 1.0f) < aggressiveness)
        {
            state = State.Attacking;
            target = enemy;
            targetController = target.gameObject.GetComponent<SoldierController>();
            gettingClose = false;
        }
        else if (Random.Range(0.0f, 1.0f) < cowardice)
        {
            state = State.Fleeing;
            target = enemy;
            targetController = target.gameObject.GetComponent<SoldierController>();
            m_agent.destination = transform.position + Quaternion.AngleAxis(Random.Range(-90, 90), Vector3.up) * (transform.position - target.position);
        }
    }

    public void HitEvent()
    {
        m_attacking = false;

        // If the target is in range
        if (target != null)
        {
            if ((target.position - transform.position).magnitude < m_agent.stoppingDistance
                && Vector3.Angle(target.position - transform.position, transform.forward) < 40)
            {
                targetController.Killed();
            }
        }
    }

    void PickNewState()
    {
        m_animator.SetBool("Fleeing", false);

        float aggressivenessBoost = 1.0f / (1.0f + (enemy.Count() / 3));
        if (Random.Range(0.0f, 1.0f) < aggressiveness + aggressivenessBoost)
        {
            state = State.Attacking;
            PickTarget();
        }
        else
        {
            state = State.Wandering;
            Vector2 wanderPosition = Random.Range(wanderRangeMin, wanderRangeMax) * Random.insideUnitCircle;
            m_agent.destination = transform.position + new Vector3(wanderPosition.x, 0, wanderPosition.y);
        }
    }

    IEnumerator PickNewStateDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        PickNewState();
    }

    public void PickTarget()
    {
        if (enemy.Count() == 0)
        {
            target = null;
        }
        else
        {
            do
            {
                targetController = enemy.GetIndex(Random.Range(0, enemy.Count()));
            } while (!enemy.Contains(targetController));
            target = targetController.transform;
            gettingClose = false;
        }
    }

}
