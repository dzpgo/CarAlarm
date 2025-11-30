namespace CarAlarm
{
    /// <summary>
    /// 车辆信息类
    /// </summary>
    internal class CarInfo
    {
        /// <summary>
        /// 车牌号
        /// </summary>
        public string? carNo { get; set; }
        /// <summary>
        /// 车主
        /// </summary>
        public string? name { get; set; }
    }
    /// <summary>
    /// 车辆进入记录用的数据类型（用于 dgvVehicleEntryLog 绑定）。
    /// </summary>
    internal class CarEntry
    {
        /// <summary>
        /// 车主
        /// </summary>
        public string? CarName { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string? CarNo { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string? Time { get; set; }
    }
}
