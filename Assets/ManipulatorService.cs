using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Manipulator;
using UnityEngine;

public class ManipulatorService : MonoBehaviour
{
    public KMService Service;
    public KMGameInfo GameInfo;

    private Dictionary<string, Func<KMBombModule, int, IEnumerable<object>>> _moduleProcessors;
    private Dictionary<string, int> _moduleCounts;
    private Coroutine _playCoroutine;

    // The values here are “ModuleType” property on the KMBombModule components.
    const string _3DMaze = "spwiz3DMaze";
    const string _3DTunnels = "3dTunnels";
    const string _AdventureGame = "spwizAdventureGame";
    const string _Alchemy = "JuckAlchemy";
    const string _Algebra = "algebra";
    const string _BigCircle = "BigCircle";
    const string _BinaryLEDs = "BinaryLeds";
    const string _Bitmaps = "BitmapsModule";
    const string _Braille = "BrailleModule";
    const string _BrokenButtons = "BrokenButtonsModule";
    const string _Bulb = "TheBulbModule";
    const string _BurglarAlarm = "burglarAlarm";
    const string _ButtonSequences = "buttonSequencesModule";
    const string _Calendar = "calendar";
    const string _CheapCheckout = "CheapCheckoutModule";
    const string _Chess = "ChessModule";
    const string _ChordQualities = "ChordQualities";
    const string _ColorDecoding = "Color Decoding";
    const string _ColoredSquares = "ColoredSquaresModule";
    const string _ColoredSwitches = "ColoredSwitchesModule";
    const string _ColorMorse = "ColorMorseModule";
    const string _Coordinates = "CoordinatesModule";
    const string _Crackbox = "CrackboxModule";
    const string _Creation = "CreationModule";
    const string _DoubleOh = "DoubleOhModule";
    const string _FastMath = "fastMath";
    const string _Functions = "qFunctions";
    const string _Gamepad = "TheGamepadModule";
    const string _GridLock = "GridlockModule";
    const string _LogicalButtons = "logicalButtonsModule";
    const string _Hexamaze = "HexamazeModule";
    const string _Hogwarts = "HogwartsModule";
    const string _HumanResources = "HumanResourcesModule";
    const string _Hunting = "hunting";
    const string _IceCream = "iceCreamModule";
    const string _Kudosudoku = "KudosudokuModule";
    const string _LEDEncryption = "LEDEnc";
    const string _Listening = "Listening";
    const string _LogicGates = "logicGates";
    const string _LondonUnderground = "londonUnderground";
    const string _ModuleMaze = "ModuleMaze";
    const string _Mafia = "MafiaModule";
    const string _MaritimeFlags = "MaritimeFlagsModule";
    const string _Microcontroller = "Microcontroller";
    const string _Minesweeper = "MinesweeperModule";
    const string _MonsplodeFight = "monsplodeFight";
    const string _MonsplodeTradingCards = "monsplodeCards";
    const string _Moon = "moon";
    const string _MorseAMaze = "MorseAMaze";
    const string _Morsematics = "MorseV2";
    const string _MouseInTheMaze = "MouseInTheMaze";
    const string _Murder = "murder";
    const string _MysticSquare = "MysticSquareModule";
    const string _Neutralization = "neutralization";
    const string _OnlyConnect = "OnlyConnectModule";
    const string _OrientationCube = "OrientationCube";
    const string _PatternCube = "PatternCubeModule";
    const string _PerspectivePegs = "spwizPerspectivePegs";
    const string _Planets = "planets";
    const string _PolyhedralMaze = "PolyhedralMazeModule";
    const string _Probing = "Probing";
    const string _Quintuples = "quintuples";
    const string _Rhythms = "MusicRhythms";
    const string _SchlagDenBomb = "qSchlagDenBomb";
    const string _SeaShells = "SeaShells";
    const string _ShapesBombs = "ShapesBombs";
    const string _ShapeShift = "shapeshift";
    const string _SillySlots = "SillySlots";
    const string _SimonSamples = "simonSamples";
    const string _SimonScreams = "SimonScreamsModule";
    const string _SimonSends = "SimonSendsModule";
    const string _SimonSings = "SimonSingsModule";
    const string _SimonSpeaks = "SimonSpeaksModule";
    const string _SimonStates = "SimonV2";
    const string _SkewedSlots = "SkewedSlotsModule";
    const string _Skyrim = "skyrim";
    const string _SonicTheHedgehog = "sonic";
    const string _Souvenir = "SouvenirModule";
    const string _Switch = "BigSwitch";
    const string _Switches = "switchModule";
    const string _SymbolCycle = "SymbolCycleModule";
    const string _SymbolicCoordinates = "symbolicCoordinates";
    const string _Synonyms = "synonyms";
    const string _TapCode = "tapCode";
    const string _TenButtonColorCode = "TenButtonColorCode";
    const string _TextField = "TextField";
    const string _ThirdBase = "ThirdBase";
    const string _TicTacToe = "TicTacToeModule";
    const string _Timezone = "timezone";
    const string _TurtleRobot = "turtleRobot";
    const string _TwoBits = "TwoBits";
    const string _UncoloredSquares = "UncoloredSquaresModule";
    const string _VisualImpairment = "visual_impairment";
    const string _Wavetapping = "Wavetapping";
    const string _Wire = "wire";
    const string _Yahtzee = "YahtzeeModule";

    void Start()
    {
        Debug.LogFormat("<Manipulator> Start()");
        _moduleProcessors = new Dictionary<string, Func<KMBombModule, int, IEnumerable<object>>>()
        {
            { _Microcontroller, ProcessMicrocontroller },
            { _Murder, ProcessMurder },
            { _Neutralization, ProcessNeutralization },
            { _PerspectivePegs, ProcessPerspectivePegs },
            { _TextField, ProcessTextField }
        };
    }

    private void OnEnable()
    {
        Debug.LogFormat("<Manipulator> OnEnable()");
        GameInfo.OnStateChange += delegate (KMGameInfo.State state)
        {
            if (state == KMGameInfo.State.Gameplay && _playCoroutine == null)
            {
                Debug.LogFormat("<Manipulator> OnStateChange -> Gameplay");
                _moduleCounts = new Dictionary<string, int>();
                _playCoroutine = StartCoroutine(Play());
            }
        };
    }

    private IEnumerator Play()
    {
        yield return new WaitForSeconds(.1f);

        Debug.LogFormat("<Manipulator> Play()");

        var modules = FindObjectsOfType<KMBombModule>();
        var names = modules.Select(m => m.ModuleDisplayName).OrderBy(m => m).ToArray();
        var expectedNames = new[] { "Battleship", "Caesar Cipher", "Perspective Pegs", "Point of Order", "Resistors", "Backgrounds", "Brush Strokes", "Brush Strokes", "Connection Check", "Curriculum", "Manometers", "Microcontroller", "Murder", "Neutralization", "Rock-Paper-Scissors-L.-Sp.", "Scripting", "Text Field", "The Screw", }.OrderBy(m => m).ToArray();
        if (names.SequenceEqual(expectedNames))
        {
            // Correct
            for (int i = 0; i < modules.Length; i++)
                StartCoroutine(ProcessModule(modules[i]));
        }
        else
        {
            // Wrong set of modules
            Debug.LogFormat("<Manipulator> Wrong set of modules on bomb:\n{0}\n\nExpected:\n{1}", names.JoinString("\n"), expectedNames.JoinString("\n"));
        }

        _playCoroutine = null;
    }

    sealed class FieldInfo<T>
    {
        private readonly object _target;
        public readonly FieldInfo Field;

        public FieldInfo(object target, FieldInfo field)
        {
            _target = target;
            Field = field;
        }

        public T Get(bool nullAllowed = false)
        {
            var t = (T) Field.GetValue(_target);
            if (!nullAllowed && t == null)
                Debug.LogFormat("<Manipulator> Field {1}.{0} is null.", Field.Name, Field.DeclaringType.FullName);
            return t;
        }

        public T GetFrom(object obj, bool nullAllowed = false)
        {
            var t = (T) Field.GetValue(obj);
            if (!nullAllowed && t == null)
                Debug.LogFormat("<Manipulator> Field {1}.{0} is null.", Field.Name, Field.DeclaringType.FullName);
            return t;
        }

        public void Set(T value) { Field.SetValue(_target, value); }
    }

    sealed class MethodInfo<T>
    {
        private readonly object _target;
        public MethodInfo Method { get; private set; }

        public MethodInfo(object target, MethodInfo method)
        {
            _target = target;
            Method = method;
        }

        public T Invoke(params object[] arguments)
        {
            return (T) Method.Invoke(_target, arguments);
        }
    }

    sealed class PropertyInfo<T>
    {
        private readonly object _target;
        public readonly PropertyInfo Property;
        public bool Error { get; private set; }

        public PropertyInfo(object target, PropertyInfo property)
        {
            _target = target;
            Property = property;
            Error = false;
        }

        public T Get(bool nullAllowed = false)
        {
            // “This value should be null for non-indexed properties.” (MSDN)
            return Get(null, nullAllowed);
        }

        public T Get(object[] index, bool nullAllowed = false)
        {
            try
            {
                var t = (T) Property.GetValue(_target, index);
                if (!nullAllowed && t == null)
                    Debug.LogFormat("<Manipulator> Property {1}.{0} is null.", Property.Name, Property.DeclaringType.FullName);
                Error = false;
                return t;
            }
            catch (Exception e)
            {
                Debug.LogFormat("<Manipulator> Property {1}.{0} could not be fetched with the specified parameters. Exception: {2}\n{3}", Property.Name, Property.DeclaringType.FullName, e.GetType().FullName, e.StackTrace);
                Error = true;
                return default(T);
            }
        }

        public void Set(T value, object[] index = null)
        {
            try
            {
                Property.SetValue(_target, value, index);
                Error = false;
            }
            catch (Exception e)
            {
                Debug.LogFormat("<Manipulator> Property {1}.{0} could not be set with the specified parameters. Exception: {2}\n{3}", Property.Name, Property.DeclaringType.FullName, e.GetType().FullName, e.StackTrace);
                Error = true;
            }
        }
    }

    private Component GetComponent(KMBombModule module, string name)
    {
        return GetComponent(module.gameObject, name);
    }
    private Component GetComponent(GameObject module, string name)
    {
        var comp = module.GetComponent(name);
        if (comp == null)
        {
            Debug.LogFormat("<Manipulator> {0} game object has no {1} component. Components are: {2}", module.name, name, module.GetComponents(typeof(Component)).Select(c => c.GetType().FullName).JoinString(", "));
            return null;
        }
        return comp;
    }

    private FieldInfo<T> GetField<T>(object target, string name, bool isPublic = false)
    {
        if (target == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} field {0} of type {2} from a null object.", name, isPublic ? "public" : "non-public", typeof(T).FullName);
            return null;
        }
        return GetFieldImpl<T>(target, target.GetType(), name, isPublic, BindingFlags.Instance);
    }

    private FieldInfo<T> GetStaticField<T>(Type targetType, string name, bool isPublic = false)
    {
        if (targetType == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} static field {2} of type {3} from a null type.", null, isPublic ? "public" : "non-public", name, typeof(T).FullName);
            return null;
        }
        return GetFieldImpl<T>(null, targetType, name, isPublic, BindingFlags.Static);
    }

    private FieldInfo<T> GetFieldImpl<T>(object target, Type targetType, string name, bool isPublic, BindingFlags bindingFlags)
    {
        var fld = targetType.GetField(name, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | bindingFlags);
        if (fld == null)
        {
            // In case it’s actually an auto-implemented property and not a field.
            fld = targetType.GetField("<" + name + ">k__BackingField", BindingFlags.NonPublic | bindingFlags);
            if (fld == null)
            {
                Debug.LogFormat("<Manipulator> Type {0} does not contain {1} field {2}. Fields are: {4}", targetType, isPublic ? "public" : "non-public", name, null,
                    targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(f => string.Format("{0} {1} {2}", f.IsPublic ? "public" : "private", f.FieldType.FullName, f.Name)).JoinString(", "));
                return null;
            }
        }
        if (!typeof(T).IsAssignableFrom(fld.FieldType))
        {
            Debug.LogFormat("<Manipulator> Type {0} has {1} field {2} of type {3} but expected type {4}.", targetType, isPublic ? "public" : "non-public", name, fld.FieldType.FullName, typeof(T).FullName);
            return null;
        }
        return new FieldInfo<T>(target, fld);
    }

    private MethodInfo<T> GetMethod<T>(object target, string name, int numParameters, bool isPublic = false)
    {
        if (target == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} method {0} of return type {2} from a null object.", name, isPublic ? "public" : "non-public", typeof(T).FullName);
            return null;
        }
        var bindingFlags = (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance;
        var targetType = target.GetType();
        var mths = targetType.GetMethods(bindingFlags).Where(m => m.Name == name && m.GetParameters().Length == numParameters && typeof(T).IsAssignableFrom(m.ReturnType)).Take(2).ToArray();
        if (mths.Length == 0)
        {
            Debug.LogFormat("<Manipulator> Type {0} does not contain {1} method {2} with return type {3} and {4} parameters.", targetType, isPublic ? "public" : "non-public", name, typeof(T).FullName, numParameters);
            return null;
        }
        if (mths.Length > 1)
        {
            Debug.LogFormat("<Manipulator> Type {0} contains multiple {1} methods {2} with return type {3} and {4} parameters.", targetType, isPublic ? "public" : "non-public", name, typeof(T).FullName, numParameters);
            return null;
        }
        return new MethodInfo<T>(target, mths[0]);
    }

    private PropertyInfo<T> GetProperty<T>(object target, string name, bool isPublic = false)
    {
        if (target == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} property {0} of type {2} from a null object.", name, isPublic ? "public" : "non-public", typeof(T).FullName);
            return null;
        }
        return GetPropertyImpl<T>(target, target.GetType(), name, isPublic, BindingFlags.Instance);
    }

    private PropertyInfo<T> GetStaticProperty<T>(Type targetType, string name, bool isPublic = false)
    {
        if (targetType == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} static property {2} of type {3} from a null type.", null, isPublic ? "public" : "non-public", name, typeof(T).FullName);
            return null;
        }
        return GetPropertyImpl<T>(null, targetType, name, isPublic, BindingFlags.Static);
    }

    private PropertyInfo<T> GetPropertyImpl<T>(object target, Type targetType, string name, bool isPublic, BindingFlags bindingFlags)
    {
        var fld = targetType.GetProperty(name, (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | bindingFlags);
        if (fld == null)
        {
            Debug.LogFormat("<Manipulator> Type {0} does not contain {1} property {2}. Properties are: {4}", targetType, isPublic ? "public" : "non-public", name, null,
                targetType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(f => string.Format("{0} {1} {2}", f.GetGetMethod().IsPublic ? "public" : "private", f.PropertyType.FullName, f.Name)).JoinString(", "));
            return null;
        }
        if (!typeof(T).IsAssignableFrom(fld.PropertyType))
        {
            Debug.LogFormat("<Manipulator> Type {0} has {1} field {2} of type {3} but expected type {4}.", targetType, isPublic ? "public" : "non-public", name, fld.PropertyType.FullName, typeof(T).FullName);
            return null;
        }
        return new PropertyInfo<T>(target, fld);
    }

    private IEnumerator ProcessModule(KMBombModule module)
    {
        var moduleType = module.ModuleType;
        _moduleCounts.IncSafe(moduleType);
        var iterator = _moduleProcessors.Get(moduleType, null);

        if (iterator != null)
        {
            Debug.LogFormat("<Manipulator> Module {0}: Start processing.", moduleType);
            foreach (var obj in iterator(module, _moduleCounts[moduleType] - 1))
                yield return obj;
            Debug.LogFormat("<Manipulator> Module {0}: Finished processing.", moduleType);
        }
        else
        {
            Debug.LogFormat("<Manipulator> Module {0}: Iterator is null.", moduleType);
        }
    }

    private IEnumerable<object> ProcessMicrocontroller(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "Micro");
        var fldSolved = GetField<int>(comp, "solved");
        var fldLedsOrder = GetField<List<int>>(comp, "LEDorder");
        var fldPositionTranslate = GetField<int[]>(comp, "positionTranslate");

        if (comp == null || fldSolved == null || fldLedsOrder == null || fldPositionTranslate == null)
            yield break;

        yield return null;

        Debug.LogFormat("<Manipulator> Microcontroller detected.");
    }

    private IEnumerable<object> ProcessMurder(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "MurderModule");
        var fldSolved = GetField<bool>(comp, "isSolved");
        var fldSolution = GetField<int[]>(comp, "solution");
        var fldNames = GetField<string[,]>(comp, "names");
        var fldSkipDisplay = GetField<int[,]>(comp, "skipDisplay");
        var fldSuspects = GetField<int>(comp, "suspects");
        var fldWeapons = GetField<int>(comp, "weapons");
        var fldBodyFound = GetField<int>(comp, "bodyFound");

        if (comp == null || fldSolved == null || fldSolution == null || fldNames == null || fldSkipDisplay == null || fldSuspects == null || fldWeapons == null || fldBodyFound == null)
            yield break;

        yield return null;

        Debug.LogFormat("<Manipulator> Murder detected.");
    }

    private IEnumerable<object> ProcessNeutralization(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "neutralization");
        var fldAcidType = GetField<int>(comp, "acidType");
        var fldAcidVol = GetField<int>(comp, "acidVol");
        var fldSolved = GetField<bool>(comp, "_isSolved");
        var fldTexts = GetField<TextMesh[]>(comp, "Text", isPublic: true);
        var fldLiquid = GetField<MeshRenderer>(comp, "liquid", isPublic: true);
        var fldFilterBtn = GetField<MeshRenderer>(comp, "filterBtn", isPublic: true);
        var fldActivated = GetField<bool>(comp, "_lightsOn");

        if (comp == null || fldAcidType == null || fldAcidVol == null || fldSolved == null || fldTexts == null || fldLiquid == null || fldFilterBtn == null || fldActivated == null)
            yield break;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(.1f);

        fldTexts.Get()[0].text = "NaOH";
        fldTexts.Get()[1].text = "18";
        fldTexts.Get()[2].text = "OFF";
        fldLiquid.Get().gameObject.SetActive(false);
        fldFilterBtn.Get().material.color = Color.red;
        module.HandlePass();
    }

    private static readonly int[] _perspectivePegsColors = new[] { 0, 2, 4, 0, 1, 3, 4, 2, 3, 1 };
    private static int GetPerspectivePegsColor(string serial)
    {
        return _perspectivePegsColors[serial.Where(ch => ch >= 'A' && ch <= 'Z').Split(2).Where(g => g.Count() == 2).Select(g => Math.Abs(g.First() - g.Last())).Sum() % 10];
    }

    private IEnumerable<object> ProcessPerspectivePegs(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "PerspectivePegsModule");
        var fldColourMeshes = GetField<MeshRenderer[,]>(comp, "ColourMeshes");
        var fldMaterials = GetField<Material[]>(comp, "Mats");
        var fldModuleID = GetField<int>(comp, "moduleId");

        if (comp == null || fldColourMeshes == null || fldMaterials == null || fldModuleID == null)
            yield break;

        var isActivated = false;
        module.OnActivate += delegate { isActivated = true; };
        while (!isActivated)
            yield return new WaitForSeconds(.1f);

        yield return null;

        // Materials: 0=red, 1=yellow, 2=green, 3=blue, 4=purple
        var materials = "RYGBP";

        // Desired arrangement
        var desired = @"RRYRR,PPRBP,GGGGP,BBYPB,YGYYB".Split(',');

        var ms = fldMaterials.Get();
        var cms = fldColourMeshes.Get();
        for (int peg = 0; peg < 5; peg++)
            for (int patch = 0; patch < 5; patch++)
                cms[peg, patch].material = ms[materials.IndexOf(desired[peg][patch])];
    }

    private IEnumerable<object> ProcessTextField(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "TextField");
        var fldDisplay = GetField<TextMesh[]>(comp, "ButtonLabels", true);
        var fldActivated = GetField<bool>(comp, "_lightson");

        if (comp == null || fldDisplay == null || fldActivated == null)
            yield break;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(0.1f);

        var displayMeshes = fldDisplay.Get();
        if (displayMeshes == null)
            yield break;

        module.HandlePass();

        for (int i = 0; i < 12; i++)
            displayMeshes[i].text = (i == 1 || i == 11) ? "✓" : "A";

        Debug.LogFormat("<Manipulator> Text Field DONE.");
    }
}
