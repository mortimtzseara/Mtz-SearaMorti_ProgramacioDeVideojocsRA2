using UnityEngine;
using UnityEngine.InputSystem;

public class CanonRotation : MonoBehaviour, InputSystem_Actions.ICannonActions
{
    public Vector3 _maxRotation;
    public Vector3 _minRotation;
    private float _offset = -51.6f;
    public GameObject ShootPoint;
    public GameObject Bullet;
    public float ProjectileSpeed = 0;
    public float MaxSpeed;
    public float MinSpeed;
    public GameObject PotencyBar;
    private float _initialScaleX;
    private Vector2 _distanceBetweenMouseAndPlayer;
    private bool isRaising = false;
    [SerializeField] private float _multiplier = 10f;
    public InputSystem_Actions inputActions;
    Vector2 worldPosition;

	private void Awake()
    {
        _initialScaleX = PotencyBar.transform.localScale.x;
		inputActions = new InputSystem_Actions();
		inputActions.Cannon.SetCallbacks(this);
    }
    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();
    void Update()
    {
        //obtenir el valor del click del cursor (Fer amb new input system)
        Vector2 mousePos = worldPosition;
        
        //obtenir el vector distància entre el canó i el cursor
        _distanceBetweenMouseAndPlayer = mousePos - (Vector2)transform.position; 

        var ang = (Mathf.Atan2(_distanceBetweenMouseAndPlayer.y, _distanceBetweenMouseAndPlayer.x) * 180f / Mathf.PI + _offset);
        ang = Mathf.Clamp(ang, _minRotation.z, _maxRotation.z);

        transform.rotation = Quaternion.Euler(0,0,ang); //en quin dels tres eixos va l'angle?
        //ShootPoint.transform.rotation = Quaternion.Euler(0, 0, ang);

        if (isRaising)
        {
            ProjectileSpeed = Time.deltaTime * _multiplier + ProjectileSpeed; //acotar entre dos valors (mirar variables)
            ProjectileSpeed = Mathf.Clamp(ProjectileSpeed, MinSpeed, MaxSpeed);
            CalculateBarScale();
        }
        
        CalculateBarScale();

    }
    public void CalculateBarScale()
    {
        PotencyBar.transform.localScale = new Vector3(Mathf.Lerp(0, _initialScaleX, ProjectileSpeed / MaxSpeed),
            transform.localScale.y,
            transform.localScale.z);
    }

    public void OnMousePosition(InputAction.CallbackContext context)
    {
        worldPosition = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }
    public void OnLeftClick(InputAction.CallbackContext context)
{
   if (context.started)
   {
       isRaising = true;
   }
   if (context.canceled)
   {
       var projectile = Instantiate(Bullet, ShootPoint.transform.position, Quaternion.identity); //canviar la posició on s'instancia
            projectile.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(_distanceBetweenMouseAndPlayer.normalized.x*ProjectileSpeed, _distanceBetweenMouseAndPlayer.normalized.y * ProjectileSpeed);
       ProjectileSpeed = 0f;
       isRaising = false;
   }
}

}
