using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpDX.DirectInput;
using XSystem.Collections;

namespace Mindream.SharpDX
{
    /// <summary>
    ///     This class manages all the inputs.
    /// </summary>
    public class InputManager
    {
        #region Constructors

        /// <summary>
        ///     Initializes the <see cref="InputManager" /> class.
        /// </summary>
        static InputManager()
        {
            Instance = new InputManager();
        }

        #endregion // Constructors.

        #region Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static InputManager Instance
        {
            get;
            private set;
        }

        #endregion // Properties.

        #region Fields

        /// <summary>
        ///     A flag to know if the reading must be continued.
        /// </summary>
        private DirectInput mManager;

        /// <summary>
        ///     A flag to know if the reading must be continued.
        /// </summary>
        private bool mContinueReading;

        /// <summary>
        ///     The keyboard
        /// </summary>
        private Keyboard mKeyboard;

        /// <summary>
        ///     The states of all the devices updates.
        /// </summary>
        private readonly Dictionary<Joystick, SynchronizedProducerConsumerCollection<List<JoystickUpdate>>> mJoystickStates = new Dictionary<Joystick, SynchronizedProducerConsumerCollection<List<JoystickUpdate>>>();

        /// <summary>
        ///     This field stores the keyboard updates.
        /// </summary>
        private SynchronizedProducerConsumerCollection<List<KeyboardUpdate>> mKeyboardUpdates;

        #endregion // Fields.

        #region Methods

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (this.mManager == null)
            {
                this.mManager = new DirectInput();
            }

            // Initialize the keyboard.
            if (this.mKeyboard == null)
            {
                this.mKeyboardUpdates = new SynchronizedProducerConsumerCollection<List<KeyboardUpdate>>();
                this.mKeyboard = new Keyboard(this.mManager);
                this.mKeyboard.Properties.BufferSize = 128;
                this.mKeyboard.Acquire();
            }

            var lThread = new Thread(this.ReadStates);
            lThread.Start();
        }

        /// <summary>
        ///     Stops this instance.
        /// </summary>
        public void Stop()
        {
            // Finalize the keyboard.
            this.mContinueReading = false;
            this.mKeyboard.Unacquire();
            this.mKeyboard = null;
        }

        /// <summary>
        ///     Adds the joystick.
        /// </summary>
        public Joystick AddJoystick()
        {
            var lInstance = this.mManager.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices).FirstOrDefault();
            if (lInstance == null)
            {
                lInstance = this.mManager.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices).FirstOrDefault();
            }

            if (lInstance != null)
            {
                var lJoystick = new Joystick(this.mManager, lInstance.InstanceGuid);
                lJoystick.Properties.BufferSize = 128;
                this.mJoystickStates.Add(lJoystick, new SynchronizedProducerConsumerCollection<List<JoystickUpdate>>());
                lJoystick.Acquire();
                return lJoystick;
            }

            return null;
        }

        /// <summary>
        ///     Gets the states.
        /// </summary>
        /// <param name="pDevice">The p device.</param>
        /// <returns></returns>
        public List<JoystickUpdate> GetJoystickStates(Joystick pDevice)
        {
            if (this.mJoystickStates.ContainsKey(pDevice))
            {
                var lStates = this.mJoystickStates[pDevice].Current.ToList();
                this.mKeyboardUpdates.Swap();
                return lStates;
            }
            return null;
        }

        /// <summary>
        ///     Gets the states.
        /// </summary>
        /// <returns>The updates of the keyboard</returns>
        public List<KeyboardUpdate> GetKeyboardState()
        {
            var lStates = this.mKeyboardUpdates.Current.ToList();
            this.mKeyboardUpdates.Swap();
            return lStates;
        }

        /// <summary>
        ///     Reads the states.
        /// </summary>
        protected void ReadStates()
        {
            // Poll all device events.
            while (true)
            {
                this.mKeyboard.Poll();
                this.mKeyboardUpdates.Next.AddRange(this.mKeyboard.GetBufferedData());

                // Iterate on joystick.
                foreach (var lJoystick in this.mJoystickStates.Keys)
                {
                    lJoystick.Poll();
                    this.mJoystickStates[lJoystick].Next.AddRange(lJoystick.GetBufferedData());
                }
            }
        }

        #endregion // Methods.
    }
}