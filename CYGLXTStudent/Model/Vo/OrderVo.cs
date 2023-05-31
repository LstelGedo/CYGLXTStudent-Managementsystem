namespace CYGLXTStudent.Model.Vo
{
    public class OrderVo : B_Order
    {
        /// <summary>
        /// 菜名
        /// </summary>
        public string Dishes { get; set; }
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuID { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int? Quantity { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 桌子编号
        /// </summary>
        public string tableNumber { get; set; }
    }
}
