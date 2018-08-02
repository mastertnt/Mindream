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
    public class RandomTracksGenerator : AComponent
    {
        #region Fields

        private Random mRandom;

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
        /// Gets the las track.
        /// </summary>
        /// <value>
        /// The las track.
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
                    this.AddTrack();
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
                        this.AddTrack();
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
                        this.SetTrackValue(lTrack);
                        this.LastTrack = lTrack;
                        if (this.TrackUpdated != null)
                        {
                            this.TrackUpdated();
                        }
                    }
                    else
                    {
                        this.AddTrack();
                    }
                }
                    break;
            }
            this.Stop();
        }


        /// <summary>
        ///     Adds a new track.
        /// </summary>
        private void AddTrack()
        {
            var lTrack = new Track();
            lTrack.Id = this.mRandom.Next(1000);
            this.SetTrackValue(lTrack);
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
        ///     Sets the track value.
        /// </summary>
        /// <param name="pTrack">The p track.</param>
        private void SetTrackValue(Track pTrack)
        {
            pTrack.Position.Latitude = this.mRandom.NextDouble() + 40;
            pTrack.Position.Longitude = this.mRandom.NextDouble() + 2;
            pTrack.Position.Altitude = this.mRandom.NextDouble()*1000 + 8000;
            pTrack.Heading = this.mRandom.NextDouble()*360;
        }

        #endregion // Methods
    }
}