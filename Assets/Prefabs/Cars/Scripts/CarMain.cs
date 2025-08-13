using TMPro;
using UnityEngine;

public class CarMain : MonoBehaviour
{

    public float speed;
    public float targetSpeedAng = 10f;



    [Header("Configuracao da Camera")]
    public GameObject cameraContainer;
    public GameObject camera;
    public Vector3 ajustePosCamera, ajusteRotCamera;
    public float referenciaTime = 3f;
    public float timeReferenciaVel = 90f;
    public float camVelTransformPos = 5f;

    [Header("Configuracao da tracao")]
    public bool tracaoTraseiras = true;
    public float torqueTraseiras = 1000f;
    public float maxAnguloTraseiras = 0;
    public float minAnguloTraseiras = 0;

    public float brakeTorqueTraseiras = 20f;

    public bool tracaoDianteiras = false;
    public float torqueDianteiras = 1000f;
    public float maxAnguloDianteiras = 30f;
    public float minAnguloDianteiras = 3f;
    public float brakeTorqueDianteiras = 20f;


    [Space]
    [Header("Configuracao Rodas")]
    public GameObject brakeLuz;
    public WheelCollider[] whelTraseiras, whelDianteiras;
    public GameObject[] meshTraseiras, meshDianteiras;


    //Inputs
     float axisVertical = 0f;
     float axisHorizontal = 0f;
     bool inputBrake = false;

    private Rigidbody rb;


    [Header("Configuracao da tracao")]
    public TextMeshProUGUI text;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camera.transform.localPosition = ajustePosCamera;
        rb.centerOfMass = new Vector3(0, -0.3f, 0);
        SpawnPos = transform.position;
        SpawnRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        axisVertical = Input.GetAxis("Vertical");
        axisHorizontal = Input.GetAxis("Horizontal");
        inputBrake = Input.GetKey(KeyCode.Space);

        UpdateWheelPose(whelTraseiras, meshTraseiras, tracaoTraseiras, torqueTraseiras, maxAnguloTraseiras, brakeTorqueDianteiras, minAnguloTraseiras);
        UpdateWheelPose(whelDianteiras, meshDianteiras, tracaoDianteiras, torqueDianteiras, maxAnguloDianteiras, brakeTorqueDianteiras, minAnguloDianteiras);

        positionCam(cameraContainer);
        moveCam(cameraContainer);
        GetSpeed();
        Respawn();
    }



    private void UpdateWheelPose(WheelCollider[] collider, GameObject[] mesh, bool isTorque, float torque, float maxAng, float brake, float minAng)
    {

        for (int i = 0; i < collider.Length; i++)
        {
            Vector3 position;
            Quaternion rotation;

            collider[i].motorTorque = isTorque ? torque * axisVertical : 0; // Torque

            collider[i].steerAngle = axisHorizontal* getAngle(maxAng, minAng); // Giro Roda

            collider[i].brakeTorque = inputBrake ? brake : 0; //Freio
            brakeLuz.SetActive(inputBrake);

            collider[i].GetWorldPose(out position, out rotation);

            mesh[i].transform.position = position;
            mesh[i].transform.rotation = rotation;    
        }

        
    }


    float getAngle(float maxAng,  float  minAng)
    {
     
        if(speed >= targetSpeedAng){
            return minAng;
        }
        return maxAng;
    }



    public void positionCam(GameObject cam)
    {
        Vector3 destino = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        cam.transform.position = Vector3.Lerp(camera.transform.position, destino, Time.deltaTime * camVelTransformPos);
    }


    public float referenceTimeCount = 0;
    public void moveCam(GameObject camera)
    {

        float sensitivity = 2.0f;

        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;


        Vector3 rotation = camera.transform.rotation.eulerAngles;

        // Converte o pitch para o intervalo -180� a 180�
        float pitch = rotation.x;
        if (pitch > 180f) pitch -= 360f;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        float yaw = rotation.y + mouseX;
        
        camera.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        referenceTimeCount += 0.01f;
        referenceTimeCount = mouseX != 0 | mouseY != 0 ? 0 : referenceTimeCount;

        if (referenceTimeCount > referenciaTime)
        {
            referenceTimeCount -= 0.01f;

            Quaternion rotacaoAtual = camera.transform.rotation;
            Quaternion rotacaoAlvoOriginal = transform.rotation;

            // Extrai apenas o Y e Z da rota��o do alvo
            Vector3 eulerAtual = rotacaoAtual.eulerAngles;
            Vector3 eulerAlvo = rotacaoAlvoOriginal.eulerAngles;

            Vector3 novaRotacao = new Vector3(
                0f,                  // mant�m o X travado
                eulerAlvo.y,         // aplica apenas Y
                eulerAlvo.z          // aplica Z se necess�rio
            );

            Quaternion rotacaoAlvoFiltrada = Quaternion.Euler(novaRotacao);

            float distanciaAngular = Quaternion.Angle(rotacaoAtual, rotacaoAlvoFiltrada);

            float velocidadeDinamica = Mathf.Clamp(distanciaAngular * timeReferenciaVel, 5f, 180f);

            // Faz a rota��o suave
            camera.transform.rotation = Quaternion.RotateTowards(rotacaoAtual, rotacaoAlvoFiltrada, velocidadeDinamica * Time.deltaTime);
        }
    }



    void GetSpeed()
    {
        if (rb == null) return;

        // Atualiza a vari�vel speed com a velocidade na dire��o forward
        speed = Vector3.Dot(rb.linearVelocity, transform.forward);
        float km = speed * 3.6f;
        int iMk = (int)km;
        text.text = iMk.ToString() + " Km";
    }

    private Vector3 SpawnPos;
    private Quaternion SpawnRot;
    void Respawn()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = SpawnPos;
            transform.rotation = SpawnRot;
        }
    }


}
