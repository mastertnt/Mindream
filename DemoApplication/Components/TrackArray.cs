using System;
using System.Collections.Generic;
using System.Linq;
using DemoApplication.Models;
using Mindream.Attributes;
using Mindream.Components;
using Mindream.Descriptors;

namespace DemoApplication.Components
{
    /// <summary>
    /// A track array generator just for debug.
    /// </summary>
    /// <seealso cref="Mindream.Components.AComponent" />
    [FunctionComponent("Generators")]
    public class TrackArray : AComponent
    {
        #region Fields

        private Random mRandom;

        private int mIndex;

        private List<Track> mTracks = new List<Track>();

        #endregion // Fields.

        #region Inputs

        /// <summary>
        ///     Gets or sets the tracks.
        /// </summary>
        /// <value>
        ///     The tracks.
        /// </value>
        [Out]
        public List<Track> Tracks
        {
            get
            {
                return this.mTracks;
            }
            set
            {
                this.mTracks = value;
            }
        }

        /// <summary>
        /// Gets the last track.
        /// </summary>
        /// <value>
        /// The last track.
        /// </value>
        [Out]
        public Track LastTrack
        {
            get;
            private set;
        }

        #endregion // Inputs

        #region Events

        /// <summary>
        /// Occurs when [track removed].
        /// </summary>
        public event ComponentReturnDelegate TrackRemoved;

        /// <summary>
        /// Occurs when [track updated].
        /// </summary>
        public event ComponentReturnDelegate TrackUpdated;

        /// <summary>
        /// Occurs when [track added].
        /// </summary>
        public event ComponentReturnDelegate TrackAdded;

        /// <summary>
        /// Occurs when [ended].
        /// </summary>
        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        /// This method is called when the component is initialized.
        /// </summary>
        protected override void ComponentInitilialized()
        {
            this.mIndex = 0;
            base.ComponentInitilialized();
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        /// <param name="pPortName">The name of the execution port to start</param>
        protected override void ComponentStarted(string pPortName)
        {
            this.mRandom = new Random(DateTime.Now.Millisecond);
            var lAction = this.mRandom.Next(0, 2);
            switch (lAction)
            {
                // Add a new track.
                case 0:
                {
                    this.AddTrack(this.Tracks.Count);
                }
                    break;

                // Remove an existing track.
                case 1:
                {
                    if (this.Tracks.Any())
                    {
                        var lIndex = this.mRandom.Next(0, this.Tracks.Count - 1);
                        this.LastTrack = this.Tracks[lIndex];
                        this.Tracks.RemoveAt(lIndex);

                        if (this.TrackRemoved != null)
                        {
                            this.TrackRemoved();
                        }
                    }
                    else
                    {
                        this.AddTrack(0);
                    }
                }
                    break;

                // Update an existing track.
                case 2:
                {
                    if (this.Tracks.Any())
                    {
                        var lIndex = this.mRandom.Next(0, this.Tracks.Count - 1);
                        var lTrack = this.Tracks[lIndex];
                        if (lIndex == 0)
                        {
                            this.SetTrackValue(lTrack, null);
                        }
                        else
                        {
                            this.SetTrackValue(lTrack, this.Tracks[lIndex - 1]);
                        }

                        this.LastTrack = lTrack;
                        if (this.TrackUpdated != null)
                        {
                            this.TrackUpdated();
                        }
                    }
                    else
                    {
                        this.AddTrack(0);
                    }
                }
                    break;
            }
            this.Stop();
        }


        /// <summary>
        ///     Adds a new track.
        /// </summary>
        private void AddTrack(int pIndex)
        {
            var lTrack = new Track();
            lTrack.Id = this.mIndex;
            this.mIndex++;
            if (pIndex != 0)
            {
                this.SetTrackValue(lTrack, this.Tracks[pIndex - 1]);
            }
            else
            {
                this.SetTrackValue(lTrack, null);
            }


            this.Tracks.Add(lTrack);
            this.LastTrack = lTrack;
            if (this.TrackAdded != null)
            {
                this.TrackAdded();
            }
        }

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStopped()
        {
            if (this.Ended != null)
            {
                this.Ended();
            }
        }

        /// <summary>
        /// Sets the track value.
        /// </summary>
        /// <param name="pTrack">The track.</param>
        /// <param name="pPreviousTrack">The previous track.</param>
        private void SetTrackValue(Track pTrack, Track pPreviousTrack)
        {
            var lPosition = new GeoPosition(0, 0, 0);
            if (pPreviousTrack != null)
            {
                lPosition = pPreviousTrack.Position;
            }

            pTrack.Position.Latitude = lPosition.Latitude + 1;
            pTrack.Position.Longitude = lPosition.Longitude + 1;
            pTrack.Position.Altitude = lPosition.Altitude;
            pTrack.Heading = this.mIndex*10%360;
        }

        #endregion // Methods
    }
}