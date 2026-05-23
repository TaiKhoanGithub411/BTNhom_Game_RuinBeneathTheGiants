namespace BTNhom.MapGen
{
    /// <summary>
    /// Enum xác định loại đối tượng có thể được sinh ra tại điểm spawn.
    /// </summary>
    public enum SpawnPointType
    {
        Item,           // Vật phẩm hồi máu, stamina, v.v.
        Trap,           // Bẫy (nấm độc, bẫy gấu, v.v.)
        Boss,           // Vị trí kích hoạt/sinh Boss trong tương lai
        Decoration      // Các vật thể trang trí tĩnh
    }
}
