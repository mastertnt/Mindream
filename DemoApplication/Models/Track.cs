using System.Text;

namespace DemoApplication.Models
{
    /// <summary>
    /// Class representing a track
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        public Track()
        {
            this.Position = new GeoPosition();
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public GeoPosition Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the heading.
        /// </summary>
        /// <value>
        /// The heading.
        /// </value>
        public double Heading
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
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