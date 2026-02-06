using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatSystem : MonoBehaviour
{
    [Header("Attack Tings")]
    [SerializeField] private float _attackSpeed = 5f;
    [SerializeField] private float  _Damage = 5f;
    [SerializeField]private float _TempattackSpeed;
   [SerializeField] private float _tempDamage;
    [SerializeField] private float _knockBack = 5f;

    [SerializeField] private GameObject _Hammer;
    [Header("Bomb Tings")]
    [SerializeField] private float _bombRadius = 3f;
    [SerializeField] private float _stunDuration = 2f;
    [Header("Sugar Rush Tings")]
    [SerializeField] private float _sugarMetter = 0f;
    [SerializeField] private float _sugarRushMax = 20f;
    [SerializeField] private float _sugarIncrease = 1f;
    [SerializeField] private float _sugarRushDuration = 10f;
    [SerializeField] private bool _canSugarRush = false;
    [SerializeField] private float _sugarCrashDuration = 10f;
    private bool _canSwing = true;
    [SerializeField]Playercontroller _playercontroller;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(_sugarMetter == _sugarRushMax)
        {
            _canSugarRush = true;

        }
        
    }

    public void hammer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            switch (_playercontroller.horizontal.x)
            {
                case -1f:
                    Debug.Log("Swing Left");
                    _Hammer.transform.SetParent(_playercontroller.leftGroundCheck);
                    _canSwing = false;

                    StartCoroutine(HammerSwing());
                    break;
                case 1:
                    Debug.Log("Swing Right");
                    _Hammer.transform.SetParent(_playercontroller.rightGroundCheck);
                    _canSwing = false;

                    StartCoroutine(HammerSwing());
                    break;
            }
            if (!_playercontroller.IsGrounded() && _playercontroller.horizontal.x == 0  && _canSwing || _playercontroller.IsGrounded() && Input.GetKey(KeyCode.DownArrow) && _playercontroller.horizontal.x == 0 && _canSwing)
            {
                _Hammer.transform.SetParent(_playercontroller.groundCheck);
                _canSwing = false;

                StartCoroutine(HammerSwing());
            }

            if (_playercontroller.IsGrounded() && _playercontroller.horizontal.x == 0 && _canSwing)
            {
                _Hammer.transform.SetParent(_playercontroller.upGroundCheck);
                _canSwing = false;

                StartCoroutine(HammerSwing());
            }

            

            _sugarMetter += _sugarIncrease;
        }
    }

    public void bomb(InputAction.CallbackContext conext)
    {
        //throw bomb in an arch
        //bombradius.enabled
        _sugarMetter += _sugarIncrease;
    }

    public void sugarRush(InputAction.CallbackContext context)
    {
        if (context.performed || _canSugarRush)
        {
            StartCoroutine(SugarRush());
        }
    }

    private IEnumerator HammerSwing()
    {
        _canSwing = false;
        switch (_playercontroller.horizontal.x)
        {
            case -1f:
                _Hammer.transform.position = _playercontroller.leftGroundCheck.position;
                _Hammer.transform.position = new Vector2(_Hammer.transform.position.x - 1, _Hammer.transform.position.y);
                _Hammer.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
                
                case 1:
                _Hammer.transform.position = _playercontroller.rightGroundCheck.position;
                _Hammer.transform.position = new Vector2(_Hammer.transform.position.x + 1, _Hammer.transform.position.y);
                _Hammer.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
                
        }

        if (_playercontroller.IsGrounded() && _playercontroller.horizontal.x == 0)
        {
            _Hammer.transform.position = _playercontroller.upGroundCheck.position;
            _Hammer.transform.position = new Vector2(_Hammer.transform.position.x, _Hammer.transform.position.y+1);
            _Hammer.transform.eulerAngles = new Vector3(0, 0, 90);
        }

       if(!_playercontroller.IsGrounded() && _playercontroller.horizontal.x == 0 || _playercontroller.IsGrounded() && Input.GetKey(KeyCode.DownArrow) && _playercontroller.horizontal.x == 0)
       {
         _Hammer.transform.position = _playercontroller.groundCheck.position;
            _Hammer.transform.position = new Vector2(_Hammer.transform.position.x, _Hammer.transform.position.y-1);
            _Hammer.transform.eulerAngles = new Vector3(0, 0, 90);

        }
        
        _Hammer.SetActive(true);
        //play the swing animation
        yield return new WaitForSeconds(_sugarIncrease);
        _Hammer.SetActive(false);
        yield return new WaitForSeconds(_sugarIncrease);
        _Hammer.transform.parent = this.transform;
        _canSwing = true;

    }
    private IEnumerator SugarRush()
    {
        _canSugarRush = false;
        if(_sugarMetter > 0f)
        {
            _tempDamage *= 2;
            _TempattackSpeed *= 2;

            yield return new WaitForSeconds(_sugarIncrease);
            _sugarMetter--;
        }

        yield return new WaitForSeconds(_sugarRushDuration);
        _tempDamage = _Damage;
        _TempattackSpeed = _attackSpeed;

        yield return new WaitForSeconds(_sugarCrashDuration);
    }
}
