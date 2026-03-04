using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; // Dönme merkezini belirleyen transform
    public float sensitivity = 10f; // Fare hassasiyeti
    public float distance = 10f; // Kameranın hedeften uzaklığı
    public float zoomSpeed = 2f; // Yakınlaştırma ve uzaklaştırma hızı
    public float minDistance = 2f; // Minimum uzaklık
    public float maxDistance = 20f; // Maksimum uzaklık

    private float _rotationX;
    private float _rotationY;

    void Start()
    {
        // Dinamik Kamera Mesafesi: Level bilgisini alıp blok boyutuna göre uzaklığı hesapla
        if (GameManager.Instance != null)
        {
            int level = GameManager.Instance.GetCurrentLevel();
            int width, height, depth, dummyRot;
            float dummyTime;
            GameManager.Instance.GetLevelSettings(level, out width, out height, out depth, out dummyRot, out dummyTime);
            
            // Eğer blok büyükse (örn: 3x3x3 -> ~10 distance, 5x5x5 -> ~18 distance)
            // Ortama göre base bir sabit sayı belirleyebiliriz:
            distance = 5f + (width * 2f); 

            // Kamera merkezini (target) bloğun tam geometrik merkezine ayarla:
            if (target != null)
            {
                float centerX = (width - 1) / 2f;
                float centerY = (height - 1) / 2f;
                float centerZ = (depth - 1) / 2f;
                target.position = new Vector3(centerX, centerY, centerZ);
            }
        }

        Vector3 relativePos = target.position - transform.position;
        if (relativePos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            _rotationY = rotation.eulerAngles.y;
            _rotationX = rotation.eulerAngles.x;
        }
    }

    void Update()
    {
        // Fare hareketinin yatay ve dikey bileşenlerini al
        float mouseHorizontalInput = Input.GetAxis("Mouse X");
        float mouseVerticalInput = Input.GetAxis("Mouse Y");

        // Yatay ve dikey dönüş açılarını hesapla
        _rotationY += mouseHorizontalInput * sensitivity;
        _rotationX -= mouseVerticalInput * sensitivity;

        // Dikey dönüş açısını sınırla (-90 ile 90 derece arasında)
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        // Kamerayı yatay ve dikey eksende döndür
        transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);

        // Yakınlaştırma ve uzaklaştırmayı gerçekleştir
        float pinchAmount = 0;
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            pinchAmount = prevTouchDeltaMag - touchDeltaMag;
        }

        // Kamerayı zoomSpeed oranında yakınlaştır veya uzaklaştır
        distance += pinchAmount * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Kamerayı objenin etrafında konumlandır
        transform.position = target.position - transform.forward * distance;
    }
}

















//using UnityEngine;

//public class OrbitCamera : MonoBehaviour
//{
//    public Transform target; // Dönme merkezini belirleyen transform
//    public float sensitivity = 10f; // Fare hassasiyeti
//    public float distance = 10f; // Kameranın hedeften uzaklığı

//    private float _rotationX;
//    private float _rotationY;

//    void Start()
//    {
//        Vector3 relativePos = target.position - transform.position;
//        Quaternion rotation = Quaternion.LookRotation(relativePos);
//        _rotationY = rotation.eulerAngles.y;
//        _rotationX = rotation.eulerAngles.x;
//    }

//    void Update()
//    {
//        // Fare hareketinin yatay ve dikey bileşenlerini al
//        float mouseHorizontalInput = Input.GetAxis("Mouse X");
//        float mouseVerticalInput = Input.GetAxis("Mouse Y");

//        // Yatay ve dikey dönüş açılarını hesapla
//        _rotationY += mouseHorizontalInput * sensitivity;
//        _rotationX -= mouseVerticalInput * sensitivity;

//        // Dikey dönüş açısını sınırla (-90 ile 90 derece arasında)
//        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

//        // Kamerayı yatay ve dikey eksende döndür
//        transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0f);

//        // Kamerayı objenin etrafında konumlandır
//        transform.position = target.position - transform.forward * distance;
//    }
//}
