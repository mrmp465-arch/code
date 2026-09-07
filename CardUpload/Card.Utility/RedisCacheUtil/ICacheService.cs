using System.Collections.Generic;
namespace Card.Utility
{
    /// <summary>
    /// Định nghĩa giao diện làm việc với cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Lấy cache theo key
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <returns>Giá trị cache</returns>
        string Get(string key);

        /// <summary>
        /// Lấy cache theo key và ép giá trị về kiểu T nếu cache không null
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <returns>Trả về đối tượng kiểu T nếu cache không null, và trà về null nếu cache null</returns>
        T Get<T>(string key);

        /// <summary>
        /// Lấy một danh sách các cache data bằng một mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        /// <returns>Danh sách key phù hợp</returns>
        List<string> GetListKeyByPattern(string pattern);

        /// <summary>
        /// Lấy một danh sách key và giá trị của key
        /// </summary>
        /// <param name="pattern">Key Mẫu</param>
        /// <returns>Danh sách key và giá trị của key</returns>
        Dictionary<string, object> GetKeyAndData(string pattern);

        /// <summary>
        /// Thêm đối tượng vào cache theo khóa, với thời gian cache là mặc định
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <param name="data">Giá trị</param>
        void Set(string key, string data);

        /// <summary>
        /// Thêm đối tượng vào cache theo khóa, với thời gian cache được chỉ định
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <param name="data">Giá trị</param>
        /// <param name="cacheTime">Thời gian cache tính theo giây</param>
        void Set(string key, string data, int cacheTime);

        /// <summary>
        /// Kiểm tra giá chị cache có được gán theo khóa cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <returns>Giá trị kiểm tra</returns>
        bool IsSet(string key);

        /// <summary>
        /// Gỡ bỏ cache theo khóa cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        void Remove(string key);

        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Xóa toàn bộ cache
        /// </summary>
        void Clear();
    }
}
