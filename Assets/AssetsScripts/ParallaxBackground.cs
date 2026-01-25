using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Настройки параллакса")]
    [SerializeField] private float parallaxEffect = 0.5f;
    [SerializeField] private bool infiniteHorizontal = true;
    [SerializeField] private bool infiniteVertical = false;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Start()
    {
        // Находим главную камеру
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        // Получаем размеры спрайта
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;

        // Вычисляем размеры в мировых координатах
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    void LateUpdate()
    {
        // Вычисляем смещение камеры
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Двигаем слой с эффектом параллакса
        transform.position += new Vector3(
            deltaMovement.x * parallaxEffect,
            deltaMovement.y * parallaxEffect * 0.5f, // Обычно вертикальный параллакс слабее
            0
        );

        lastCameraPosition = cameraTransform.position;

        // Бесконечная прокрутка по горизонтали
        if (infiniteHorizontal && Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cameraTransform.position.x + offsetX, transform.position.y);
        }

        // Бесконечная прокрутка по вертикали
        if (infiniteVertical && Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureUnitSizeY)
        {
            float offsetY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetY);
        }
    }
}   