using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DemoApplication.Models;
using Mindream;
using Mindream.Attributes;
using Mindream.Components;

namespace DemoApplication.Components
{
    [FunctionComponent]
    public class RandomTracksGenerator : AFunctionComponent
    {
        #region Fields

        private DateTime mStartTime;

        private Random mRandom;

        private List<Track> mTracks = new List<Track>(); 

        #endregion // Fields.

        #region Inputs

        [In]
        public TimeSpan UpdatePeriod { get; set; }

        [In]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gets or sets the tracks.
        /// </summary>
        /// <value>
        /// The tracks.
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

        #endregion // Inputs

        #region Events

        public event ComponentReturnDelegate TrackRemoved;

        public event ComponentReturnDelegate TrackUpdated;

        public event ComponentReturnDelegate TrackAdded;

        public event ComponentReturnDelegate Ended;

        #endregion // Events.

        #region Methods

        /// <summary>
        ///     This method is called to start the component.
        /// </summary>
        protected override void ComponentStarted()
        {
            this.mStartTime = DateTime.Now;
            Timer lTimer = new Timer {Interval = this.UpdatePeriod.TotalMilliseconds};
            lTimer.Elapsed += this.OnTimerElapsed;
            lTimer.Enabled = true;
            lTimer.AutoReset = false;
            this.mRandom = new Random(DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Called when [timer elapsed].
        /// </summary>
        /// <param name="pSender">The p sender.</param>
        /// <param name="pEventArgs">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnTimerElapsed(object pSender, ElapsedEventArgs pEventArgs)
        {
            TimeSpan lDiff = DateTime.Now - this.mStartTime;
            if (lDiff.TotalMilliseconds > this.Duration.TotalMilliseconds)
            {
                Timer lTimer = (Timer) pSender;
                lTimer.Enabled = false;
                lTimer.Elapsed -= this.OnTimerElapsed;
                this.Stop();
            }
            else
            {
                int lAction = this.mRandom.Next(0, 2);
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
                            int lIndex = this.mRandom.Next(0, this.Tracks.Count - 1);
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
                            int lIndex = this.mRandom.Next(0, this.Tracks.Count - 1);
                            Track lTrack = this.Tracks[lIndex];
                            SetTrackValue(lTrack);

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
                Timer lTimer = (Timer)pSender;
                lTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Adds a new track.
        /// </summary>
        private void AddTrack()
        {
            Track lTrack = new Track();
            SetTrackValue(lTrack);
            this.Tracks.Add(lTrack);
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
        /// <param name="pTrack">The p track.</param>
        private void SetTrackValue(Track pTrack)
        {
            pTrack.Position.Latitude = this.mRandom.NextDouble() + 40;
            pTrack.Position.Longitude = this.mRandom.NextDouble() + 2;
            pTrack.Position.Altitude = this.mRandom.NextDouble() * 1000 + 8000;
            pTrack.Heading = this.mRandom.NextDouble() * 360;
        }

        #endregion // Methods
    }
}
