namespace BallDataNS
{
    public class BallData
    {
        private int posCol;
        private int posRow;
        private bool isSmall;

        BallData()
        {
            this.PosCol = 0;
            this.PosRow = 0;
            this.isSmall = true;
        }
        BallData(int posCol, int posRow)
        {
            this.PosCol = posCol;
            this.PosRow = posRow;
        }
        BallData(int posCol, int posRow, bool isSmall)
        {
            this.PosCol = posCol;
            this.PosRow = posRow;
            this.isSmall = isSmall;
        }

        public int PosCol { get => posCol; set => posCol = value; }
        public int PosRow { get => posRow; set => posRow = value; }

        //click vào quả bóng
        //truyền col và row của quả bóng cho grid Plate, gọi hàm trong gamemanager
        //find game object with name của palete
        //set trang thái của game sang clicking
        //chờ click chổ trống thứ 2.
        //tính đường đi, nếu không có hoặc palete isFill thì vẫn duy trì trạng thái clicking.
        //nếu có đường đi thỏa mãn thì, moving quả bóng đến palete mới.
        //set trạng thái palete củ là isFill = true.
    }
}