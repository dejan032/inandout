namespace InAndOut.ViewModel.MVVMMSGS
{
    public class UpdateMenuItems
    {
        private string className;

        public UpdateMenuItems(string className)
        {
            this.className = className;
        }

        public string ClassName => className;
    }
}