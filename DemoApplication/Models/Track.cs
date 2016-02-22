using System.Text;

namespace DemoApplication.Models
{
    public class Track
    {
        public Track()
        {
            this.Position = new GeoPosition();
        }

        public int Id
        {
            get;
            set;
        }

        public GeoPosition Position
        {
            get;
            set;
        }

        public double Heading
        {
            get;
            set;
        }

        public override string ToString()
        {
            var lBuilder = new StringBuilder();

            lBuilder.Append("Track ");
            lBuilder.Append(this.Id);
            lBuilder.Append(" Pos " + this.Position);

            return lBuilder.ToString();
        }
    }
}