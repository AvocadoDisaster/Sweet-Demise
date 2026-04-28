using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class CombatSystem : MonoBehaviour
{
    
    [Header("Attack Settings")]
    [SerializeField] private float _attackSpeed = 5f;
    [SerializeField] private float _damage = 5f;
    [SerializeField] private float _knockBack = 5f;
    [SerializeField] private float _attackCooldown = 0.5f;   

    
    private float _tempAttackSpeed;
    private float _tempDamage;


    [Header("Hammer")]
    [SerializeField] private GameObject _hammer;
    [SerializeField] private string _hammerDamageTag = "Enemy"; 

    
    [SerializeField] private GameObject _bombPrefab;

    
    [Header("Spread Shot")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletForce = 20f;
    [SerializeField] private int _numberOfProjectiles = 5;
    [Range(0, 360)]
    [SerializeField] private float _totalSpreadAngle = 30f;
    [SerializeField] private int _bulletDamage = 10;   

    
    [Header("Sugar Rush")]
    [SerializeField] private float _sugarMeter = 0f;
    [SerializeField] private float _sugarRushMax = 20f;
    [SerializeField] private float _sugarIncrease = 1f;
    [SerializeField] private float _sugarRushDuration = 10f;
    [SerializeField] private float _sugarCrashDuration = 10f;
    [SerializeField] private bool _canSugarRush = false;

    
    [Header("References")]
    [SerializeField] private Playercontroller _playerController;
    public LifeTime LifeTime;

   
    private bool _canSwing = true;
    private bool _inSugarRush = false;

    
    private void Start()
    {
        _tempDamage = _damage;
        _tempAttackSpeed = _attackSpeed;
    }

    private void Update()
    {
        
        _canSugarRush = (_sugarMeter >= _sugarRushMax) && !_inSugarRush;

        
        if (LifeTime != null && LifeTime.lifetiming == 0)
            LifeTime.lifetiming = LifeTime.originallife;
    }

   
    public void hammer(InputAction.CallbackContext context)
    {
        if (!context.performed || !_canSwing) return;

      
        switch (_playerController.horizontal.x)
        {
            case -1f:
                _hammer.transform.SetParent(_playerController.leftGroundCheck);
                StartCoroutine(HammerSwing());
                break;
            case 1f:
                _hammer.transform.SetParent(_playerController.rightGroundCheck);
                StartCoroutine(HammerSwing());
                break;
            default:
                bool goingDown = !_playerController.IsGrounded() ||
                                 (Input.GetKey(KeyCode.DownArrow) && _playerController.horizontal.x == 0);

                _hammer.transform.SetParent(goingDown
                    ? _playerController.groundCheck
                    : _playerController.upGroundCheck);
                StartCoroutine(HammerSwing());
                break;
        }

        GainSugar(_sugarIncrease);
        StartCoroutine(SpreadShot());
    }

    
    public void bomb(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_bombPrefab != null)
        {
            GameObject b = Instantiate(_bombPrefab, _hammer.transform.position, Quaternion.identity);

            
            BombDamage bd = b.GetComponent<BombDamage>();
            if (bd != null) bd.damage = Mathf.RoundToInt(_tempDamage);
        }

        GainSugar(_sugarIncrease);
    }


    public void sugarRush(InputAction.CallbackContext context)
    {
        if (context.performed && _canSugarRush)
            StartCoroutine(SugarRush());
    }

    

    private IEnumerator HammerSwing()
    {
        _canSwing = false;

       
        PositionHammer();

        _hammer.SetActive(true);

      
        yield return new WaitForSeconds(1f / _tempAttackSpeed);

        _hammer.SetActive(false);
        yield return new WaitForSeconds(_attackCooldown);

        _hammer.transform.SetParent(this.transform);
        _canSwing = true;
    }

    private IEnumerator SugarRush()
    {
        _inSugarRush = true;
        _canSugarRush = false;

        _tempDamage = _damage * 2f;
        _tempAttackSpeed = _attackSpeed * 2f;

        float drainPerTick = _sugarRushMax / (_sugarRushDuration / 0.1f);
        while (_sugarMeter > 0f)
        {
            _sugarMeter = Mathf.Max(0f, _sugarMeter - drainPerTick);
            yield return new WaitForSeconds(0.1f);
        }

       
        _tempDamage = _damage * 0.5f;
        _tempAttackSpeed = _attackSpeed * 0.5f;
        yield return new WaitForSeconds(_sugarCrashDuration);

       
        _tempDamage = _damage;
        _tempAttackSpeed = _attackSpeed;
        _inSugarRush = false;
    }

    private IEnumerator SpreadShot()
    {
        if (_bulletPrefab == null || _firePoint == null) yield break;

        float angleStep = _numberOfProjectiles > 1
            ? _totalSpreadAngle / (_numberOfProjectiles - 1)
            : 0f;
        float centeringOffset = _totalSpreadAngle / 2f;

        for (int i = 0; i < _numberOfProjectiles; i++)
        {
            float angle = angleStep * i - centeringOffset;
            Quaternion rot = Quaternion.Euler(0, 0, _firePoint.eulerAngles.z + angle);
            GameObject b = Instantiate(_bulletPrefab, _firePoint.position, rot);

         
            BossProjectile bp = b.GetComponent<BossProjectile>();
            if (bp != null)
            {
                bp.damage = Mathf.RoundToInt(_tempDamage + _bulletDamage);
                bp.targetTag = "Enemy";
            }

        
            Rigidbody2D rb = b.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float dir = _playerController.horizontal.x >= 0 ? 1f : -1f;
                rb.AddForce(b.transform.right * dir * _bulletForce, ForceMode2D.Impulse);
            }

            if (Input.GetKey(KeyCode.DownArrow) && LifeTime != null)
                LifeTime.lifetiming = 0f;
        }

        yield return new WaitForSeconds(0.1f);
    }

   

    private void PositionHammer()
    {
        switch (_playerController.horizontal.x)
        {
            case -1f:
                _hammer.transform.position = (Vector2)_playerController.leftGroundCheck.position
                                             + Vector2.left;
                _hammer.transform.eulerAngles = Vector3.zero;
                break;

            case 1f:
                _hammer.transform.position = (Vector2)_playerController.rightGroundCheck.position
                                             + Vector2.right;
                _hammer.transform.eulerAngles = Vector3.zero;
                break;

            default:
                bool down = !_playerController.IsGrounded() ||
                            (Input.GetKey(KeyCode.DownArrow) && _playerController.horizontal.x == 0);
                if (down)
                {
                    _hammer.transform.position = (Vector2)_playerController.groundCheck.position
                                                 + Vector2.down;
                    _hammer.transform.eulerAngles = new Vector3(0, 0, 90);
                }
                else
                {
                    _hammer.transform.position = (Vector2)_playerController.upGroundCheck.position
                                                 + Vector2.up;
                    _hammer.transform.eulerAngles = new Vector3(0, 0, 90);
                }
                break;
        }
    }

    private void GainSugar(float amount)
    {
        if (_inSugarRush) return;   
        _sugarMeter = Mathf.Min(_sugarRushMax, _sugarMeter + amount);
    }

}






