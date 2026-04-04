using StackExchange.Redis;

namespace NilArea.Contracts;

public static partial class Extensions
{
    /// <summary>
    ///     Redis布隆过滤器扩展方法
    ///     提供对Redis Bloom Filter数据结构的异步操作支持
    /// </summary>
    /// <remarks>
    ///     Bloom Filter是一种空间效率高的概率性数据结构，用于判断一个元素是否在集合中
    ///     特点：可能误报（false positive），但不会漏报（false negative）
    /// </remarks>
    extension(IDatabaseAsync db)
    {
        /// <summary>
        ///     创建一个新的布隆过滤器
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <param name="errorRate">
        ///     期望的错误率（误报率），范围：(0, 1]
        ///     <para>例如：0.01 表示1%的错误率</para>
        /// </param>
        /// <param name="initialCapacity">
        ///     预期要插入的元素数量
        ///     <para>注意：实际容量会根据错误率自动调整</para>
        /// </param>
        /// <returns>表示异步操作的任务</returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" /> 或 <paramref name="key" /> 为null时抛出
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     当 <paramref name="errorRate" /> 不在 (0, 1] 范围内时抛出
        ///     当 <paramref name="initialCapacity" /> 小于等于0时抛出
        /// </exception>
        /// <exception cref="RedisException">
        ///     当Redis命令执行失败时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     布隆过滤器已经存在
        /// </exception>
        /// <example>
        ///     <code>
        /// // 创建一个错误率为1%，初始容量为1000的布隆过滤器
        /// await db.BloomReserveAsync("user:filter", 0.01, 1000);
        /// </code>
        /// </example>
        public async Task BloomReserveAsync(
            RedisKey key,
            double errorRate,
            int initialCapacity)
        {
            ArgumentNullException.ThrowIfNull(db);
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            if (errorRate is <= 0 or > 1)
                throw new ArgumentOutOfRangeException(nameof(errorRate), errorRate, "错误率必须在 (0, 1] 范围内");
            if (initialCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), initialCapacity, "初始容量必须大于0");
            try
            {
                await db.ExecuteAsync("BF.RESERVE", key, errorRate, initialCapacity).ConfigureAwait(false);
            }
            catch (RedisServerException ex) when (ex.Message.Contains("ERR"))
            {
                // 特定错误处理
                if (ex.Message.Contains("item exists")) throw new InvalidOperationException($"布隆过滤器已存在，键: {key}", ex);
                throw new RedisException($"创建布隆过滤器失败，键: {key}, 错误率: {errorRate}, 容量: {initialCapacity}", ex);
            }
        }

        /// <summary>
        ///     向布隆过滤器中添加一个元素
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <param name="value">要添加的元素值</param>
        /// <returns>
        ///     表示异步操作的任务，返回一个布尔值：
        ///     <para>true - 元素是首次添加（之前不存在）</para>
        ///     <para>false - 元素可能已经存在（可能是误报）</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" />、<paramref name="key" /> 或 <paramref name="value" /> 为null时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当指定的布隆过滤器不存在时抛出
        /// </exception>
        /// <exception cref="RedisException">
        ///     当Redis命令执行失败时抛出
        /// </exception>
        /// <remarks>
        ///     注意：返回false并不一定表示元素已经存在，可能是布隆过滤器的误报
        /// </remarks>
        /// <example>
        ///     <code>
        /// // 添加一个用户ID到布隆过滤器
        /// bool isNew = await db.BloomAddAsync("user:filter", "user123");
        /// if (isNew)
        /// {
        ///     Console.WriteLine("可能是新用户");
        /// }
        /// </code>
        /// </example>
        public async Task<bool> BloomAddAsync(
            RedisKey key,
            RedisValue value)
        {
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            if (value == default || value.IsNull)
                throw new ArgumentNullException(nameof(value));
            try
            {
                var result = await db.ExecuteAsync("BF.ADD", key, value).ConfigureAwait(false);
                return !result.IsNull && (bool)result;
            }
            catch (RedisServerException ex) when (ex.Message.Contains("not found"))
            {
                throw new InvalidOperationException($"布隆过滤器不存在，请先使用 BloomReserveAsync 创建，键: {key}", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new RedisException($"Redis响应类型转换失败，期望类型: bool, 键: {key}, 值: {value}", ex);
            }
        }

        /// <summary>
        ///     向布隆过滤器中批量添加多个元素
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <param name="values">要添加的元素值集合</param>
        /// <returns>
        ///     表示异步操作的任务，返回布尔数组：
        ///     <para>数组中的每个元素对应输入集合中相应位置的元素添加结果</para>
        ///     <para>true - 元素是首次添加（之前不存在）</para>
        ///     <para>false - 元素可能已经存在（可能是误报）</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" />、<paramref name="key" /> 或 <paramref name="values" /> 为null时抛出
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     当 <paramref name="values" /> 集合为空时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当指定的布隆过滤器不存在时抛出
        /// </exception>
        /// <exception cref="RedisException">
        ///     当Redis命令执行失败时抛出
        /// </exception>
        /// <example>
        ///     <code>
        /// // 批量添加用户ID
        /// var userIds = new[] { "user1", "user2", "user3" };
        /// bool[] results = await db.BloomAddMultipleAsync("user:filter", userIds);
        /// 
        /// for (int i = 0; i &lt; results.Length; i++)
        /// {
        ///     Console.WriteLine($"用户 {userIds[i]} 是否新添加: {results[i]}");
        /// }
        /// </code>
        /// </example>
        public async Task<bool[]> BloomAddMultipleAsync(
            RedisKey key,
            params IEnumerable<RedisValue> values)
        {
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(values);

            var valueList = values.ToArray();
            if (valueList.Length == 0)
                throw new ArgumentException("值集合不能为空", nameof(values));
            try
            {
                var result = await db.ExecuteAsync("BF.MADD", valueList.Cast<object>().Prepend(key).ToArray())
                    .ConfigureAwait(false);
                if (result.IsNull || result.Length < 0) return [];
                return Enumerable.Range(0, result.Length).Select(i => (bool)result[i]).ToArray();
            }
            catch (RedisServerException ex) when (ex.Message.Contains("not found"))
            {
                throw new InvalidOperationException($"布隆过滤器不存在，请先使用 BloomReserveAsync 创建，键: {key}", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new RedisException($"Redis响应类型转换失败，期望类型: bool[], 键: {key}, 值数量: {valueList.Length}", ex);
            }
        }


        /// <summary>
        ///     检查一个元素是否可能存在于布隆过滤器中
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <param name="value">要检查的元素值</param>
        /// <returns>
        ///     表示异步操作的任务，返回一个布尔值：
        ///     <para>true - 元素可能存在于过滤器中（可能是误报）</para>
        ///     <para>false - 元素绝对不存在于过滤器中</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" />、<paramref name="key" /> 或 <paramref name="value" /> 为null时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当指定的布隆过滤器不存在时抛出
        /// </exception>
        /// <exception cref="RedisException">
        ///     当Redis命令执行失败时抛出
        /// </exception>
        /// <remarks>
        ///     重要：返回true可能是误报，但返回false一定是准确的不存在
        ///     适用于"不存在性检查"场景，如缓存穿透防护
        /// </remarks>
        /// <example>
        ///     <code>
        /// // 检查用户ID是否可能已存在
        /// bool possiblyExists = await db.BloomExistsAsync("user:filter", "user123");
        /// if (!possiblyExists)
        /// {
        ///     // 绝对不存在，可以安全进行数据库查询
        ///     var user = await userRepository.GetAsync("user123");
        /// }
        /// </code>
        /// </example>
        public async Task<bool> BloomExistsAsync(
            RedisKey key,
            RedisValue value)
        {
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            if (value == default || value.IsNull)
                throw new ArgumentNullException(nameof(value));
            try
            {
                var result = await db.ExecuteAsync("BF.EXISTS", key, value)
                    .ConfigureAwait(false);
                return !result.IsNull && (bool)result;
            }
            catch (RedisServerException ex) when (ex.Message.Contains("not found"))
            {
                throw new InvalidOperationException($"布隆过滤器不存在，键: {key}", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new RedisException($"Redis响应类型转换失败，期望类型: bool, 键: {key}, 值: {value}", ex);
            }
        }

        /// <summary>
        ///     批量检查多个元素是否可能存在于布隆过滤器中
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <param name="values">要检查的元素值集合</param>
        /// <returns>
        ///     表示异步操作的任务，返回布尔数组：
        ///     <para>数组中的每个元素对应输入集合中相应位置的元素检查结果</para>
        ///     <para>true - 元素可能存在于过滤器中（可能是误报）</para>
        ///     <para>false - 元素绝对不存在于过滤器中</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" />、<paramref name="key" /> 或 <paramref name="values" /> 为null时抛出
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     当 <paramref name="values" /> 集合为空时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当指定的布隆过滤器不存在时抛出
        /// </exception>
        /// <exception cref="RedisException">
        ///     当Redis命令执行失败时抛出
        /// </exception>
        /// <example>
        ///     <code>
        /// // 批量检查用户ID
        /// var userIds = new[] { "user1", "user2", "user3" };
        /// bool[] existsResults = await db.BloomExistsMultipleAsync("user:filter", userIds);
        /// 
        /// // 只查询绝对不存在的用户ID
        /// var nonExistingIds = userIds
        ///     .Zip(existsResults, (id, exists) => new { id, exists })
        ///     .Where(x => !x.exists)
        ///     .Select(x => x.id)
        ///     .ToList();
        /// </code>
        /// </example>
        public async Task<bool[]> BloomExistsMultipleAsync(
            RedisKey key,
            IEnumerable<RedisValue> values)
        {
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(values);
            var valueList = values.ToArray();
            if (valueList.Length == 0)
                throw new ArgumentException("值集合不能为空", nameof(values));
            try
            {
                var result = await db.ExecuteAsync("BF.MEXISTS", valueList.Cast<object>().Prepend(key).ToArray())
                    .ConfigureAwait(false);
                if (result.IsNull || result.Length < 0) return new bool[valueList.Length];
                return Enumerable.Range(0, result.Length).Select(i => (bool)result[i]).ToArray();
            }
            catch (RedisServerException ex) when (ex.Message.Contains("not found"))
            {
                throw new InvalidOperationException($"布隆过滤器不存在，键: {key}", ex);
            }
            catch (InvalidCastException ex)
            {
                throw new RedisException($"Redis响应类型转换失败，期望类型: bool[], 键: {key}, 值数量: {valueList.Length}", ex);
            }
        }

        /// <summary>
        ///     获取布隆过滤器的信息
        /// </summary>
        /// <param name="key">布隆过滤器的Redis键</param>
        /// <returns>
        ///     表示异步操作的任务，返回布隆过滤器信息对象
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     当 <paramref name="db" /> 或 <paramref name="key" /> 为null时抛出
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     当指定的布隆过滤器不存在时抛出
        /// </exception>
        /// <remarks>
        ///     需要RedisBloom模块支持 BF.INFO 命令
        /// </remarks>
        public async Task<BloomFilterInfo?> BloomInfoAsync(
            RedisKey key)
        {
            if (key == default(RedisKey))
                throw new ArgumentNullException(nameof(key));
            try
            {
                var result = await db.ExecuteAsync("BF.INFO", key)
                    .ConfigureAwait(false);
                if (result.IsNull || result.Length < 0) return null;
                var infoArray = Enumerable.Range(0, result.Length).Select(i => result[i]).Cast<object>().ToArray();
                // 解析Redis返回的信息数组
                var info = new BloomFilterInfo();
                for (var i = 0; i < infoArray.Length - 1; i += 2)
                {
                    var property = infoArray[i] as string;
                    var value = infoArray[i + 1];
                    switch (property?.ToUpperInvariant())
                    {
                        case "CAPACITY":
                            info.Capacity = Convert.ToInt32(value);
                            break;
                        case "SIZE":
                            info.Size = Convert.ToInt64(value);
                            break;
                        case "NUMBER OF FILTERS":
                            info.NumberOfFilters = Convert.ToInt32(value);
                            break;
                        case "NUMBER OF ITEMS INSERTED":
                            info.ItemsInserted = Convert.ToInt64(value);
                            break;
                        case "EXPANSION RATE":
                            info.ExpansionRate = Convert.ToInt32(value);
                            break;
                    }
                }

                return info;
            }
            catch (RedisServerException ex) when (ex.Message.Contains("unknown command"))
            {
                throw new NotSupportedException("Redis服务器不支持 BF.INFO 命令，请确保已加载RedisBloom模块", ex);
            }
        }
    }

    /// <summary>
    ///     布隆过滤器信息
    /// </summary>
    public record BloomFilterInfo
    {
        /// <summary>
        ///     过滤器容量（创建时指定的初始容量）
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        ///     过滤器大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        ///     过滤器数量（当过滤器自动扩展时）
        /// </summary>
        public int NumberOfFilters { get; set; }

        /// <summary>
        ///     已插入的元素数量
        /// </summary>
        public long ItemsInserted { get; set; }

        /// <summary>
        ///     扩展率（过滤器自动扩展时的增长因子）
        /// </summary>
        public int ExpansionRate { get; set; }

        /// <summary>
        ///     计算当前错误率估计值
        /// </summary>
        /// <returns>估计的错误率（误报率）</returns>
        /// <remarks>
        ///     这是一个近似计算，实际错误率可能略有不同
        ///     公式：p = (1 - e^(-k * n / m)) ^ k
        ///     其中：k = 哈希函数数量，n = 已插入元素数量，m = 位数组大小
        /// </remarks>
        public double EstimateErrorRate()
        {
            if (Capacity <= 0 || ItemsInserted <= 0)
                return 0;

            // 简化计算：使用标准公式估算
            // 假设哈希函数数量 k = 0.7 * (m / n)
            var m = Capacity * 8.0; // 位数量（假设每个字节8位）
            var n = ItemsInserted;
            var k = Math.Round(0.7 * m / n);
            if (k < 1) k = 1;
            return Math.Pow(1 - Math.Exp(-k * n / m), k);
        }

        /// <summary>
        ///     获取信息字符串表示
        /// </summary>
        public override string ToString()
        {
            return $"Capacity: {Capacity}, Size: {Size} bytes, " +
                   $"Items: {ItemsInserted}, Filters: {NumberOfFilters}, " +
                   $"Expansion: {ExpansionRate}, " +
                   $"Estimated Error Rate: {EstimateErrorRate():P2}";
        }
    }
}