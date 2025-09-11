// IInitializable.cs
public interface IInitializable
{
    // GameManager로부터 필요한 데이터를 받아 스스로를 초기화합니다.
    void Initialize(GameManager gameManager);
}
