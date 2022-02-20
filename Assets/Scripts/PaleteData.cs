namespace PaleteDataNS
{
    public class PaleteData
    {
        private int posRow;
        private int posCol;
        private bool isFill;

        public int PosRow { get => posRow; set => posRow = value; }
        public int PosCol { get => posCol; set => posCol = value; }

        public PaleteData()
        {
            this.posCol = 0;
            this.posRow = 0;
            this.isFill = false;
        }
        public PaleteData(int posCol, int posRow)
        {
            this.posRow = posRow;
            this.posCol = posCol;
            this.isFill = false;
        }

        public void SetPos(int col, int row)
        {
            this.PosCol = col;
            this.PosRow = row;
        }

        public void SetFill(bool isFill)
        {
            this.isFill = isFill;
        }
        public bool GetFill()
        {
            return this.isFill;
        }
    }
}