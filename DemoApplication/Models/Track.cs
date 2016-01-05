namespace DemoApplication.Models
{
    public class Track
    {
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

        public Track()
        {
            this.Position = new GeoPosition();
        }
    }
}
