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
    public GameObject MysteryWidgetPrefab;

    private Dictionary<string, Func<KMBombModule, int, IEnumerable<object>>> _moduleProcessors;
    private Dictionary<string, int> _moduleCounts;
    private Coroutine _playCoroutine;

    // The values here are “ModuleType” property on the KMBombModule components.
    const string _3DMaze = "spwiz3DMaze";
    const string _3DTunnels = "3dTunnels";
    const string _AdventureGame = "spwizAdventureGame";
    const string _Alchemy = "JuckAlchemy";
    const string _Algebra = "algebra";
    const string _Backgrounds = "Backgrounds";
    const string _Battleship = "BattleshipModule";
    const string _BigCircle = "BigCircle";
    const string _BinaryLEDs = "BinaryLeds";
    const string _Bitmaps = "BitmapsModule";
    const string _Braille = "BrailleModule";
    const string _BrokenButtons = "BrokenButtonsModule";
    const string _BrushStrokes = "brushStrokes";
    const string _Bulb = "TheBulbModule";
    const string _BurglarAlarm = "burglarAlarm";
    const string _ButtonSequences = "buttonSequencesModule";
    const string _CaesarCipher = "CaesarCipherModule";
    const string _Calendar = "calendar";
    const string _CheapCheckout = "CheapCheckoutModule";
    const string _Chess = "ChessModule";
    const string _ChordQualities = "ChordQualities";
    const string _ColorDecoding = "Color Decoding";
    const string _ColoredSquares = "ColoredSquaresModule";
    const string _ColoredSwitches = "ColoredSwitchesModule";
    const string _ColorMorse = "ColorMorseModule";
    const string _ConnectionCheck = "graphModule";
    const string _Coordinates = "CoordinatesModule";
    const string _Crackbox = "CrackboxModule";
    const string _Creation = "CreationModule";
    const string _Curriculum = "curriculum";
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
    const string _Manometers = "manometers";
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
    const string _PointOfOrder = "PointOfOrderModule";
    const string _PolyhedralMaze = "PolyhedralMazeModule";
    const string _Probing = "Probing";
    const string _Quintuples = "quintuples";
    const string _Resistors = "resistors";
    const string _Rhythms = "MusicRhythms";
    const string _RockPaperScissorsLizardSpock = "RockPaperScissorsLizardSpockModule";
    const string _SchlagDenBomb = "qSchlagDenBomb";
    const string _Screw = "screw";
    const string _Scripting = "KritScripts";
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
            { _Backgrounds, ProcessBackgrounds },
            { _Battleship, ProcessBattleship },
            { _BrushStrokes, ProcessBrushStrokes },
            { _CaesarCipher, ProcessCaesarCipher },
            { _ConnectionCheck, ProcessConnectionCheck },
            { _Curriculum, ProcessCurriculum },
            { _Manometers, ProcessManometers },
            { _Microcontroller, ProcessMicrocontroller },
            { _Murder, ProcessMurder },
            { _Neutralization, ProcessNeutralization },
            { _OnlyConnect, ProcessOnlyConnect },
            { _PerspectivePegs, ProcessPerspectivePegs },
            { _PointOfOrder, ProcessPointOfOrder },
            { _Resistors, ProcessResistors },
            { _RockPaperScissorsLizardSpock, ProcessRockPaperScissorsLizardSpock },
            { _Screw, ProcessScrew },
            { _Scripting, ProcessScripting },
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
        var expectedNames = new[] { "Battleship", "Caesar Cipher", "Point of Order", "Resistors", "Backgrounds", "Brush Strokes", "Brush Strokes", "Connection Check", "Manometers", "Microcontroller", "Murder", "Neutralization", "Only Connect", "Scripting", "Text Field", "The Screw", }.OrderBy(m => m).ToArray();
        if (names.SequenceEqual(expectedNames))
        {
            // Correct
            for (int i = 0; i < modules.Length; i++)
                StartCoroutine(ProcessModule(modules[i]));
            StartCoroutine(RemoveEdgework());
        }
        else
        {
            // Wrong set of modules
            Debug.LogFormat("<Manipulator> Wrong set of modules on bomb:\n{0}\n\nExpected:\n{1}", names.JoinString("\n"), expectedNames.JoinString("\n"));
        }

        _playCoroutine = null;
    }

    private IEnumerator RemoveEdgework()
    {
        yield return new WaitForSeconds(2.5f);
        var widgetAreaParent = FindObjectOfType<KMBombModule>().transform.parent.Find("WidgetAreas");
        for (int i = 0; i < widgetAreaParent.childCount; i++)
        {
            var widgetArea = widgetAreaParent.GetChild(i);

            for (int j = 0; j < widgetArea.childCount; j++)
            {
                var widget = widgetArea.GetChild(j).gameObject;
                if (widget.name.StartsWith("SerialNumber"))
                {
                    var label = widget.transform.Find("SerialText").GetComponent("TextMeshPro");
                    if (label == null)
                        Debug.LogFormat("<Manipulator> Serial number: label is null!");
                    else
                    {
                        var prop = label.GetType().GetProperty("text", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (prop == null)
                            Debug.LogFormat("<Manipulator> Serial number: prop is null!");
                        else
                            prop.SetValue(label, "??????", null);
                    }
                    Debug.LogFormat("<Manipulator> Serial number processed.");
                }
                else if (widget.name.StartsWith("PortWidget") || widget.name.StartsWith("IndicatorWidget") || widget.name.StartsWith("BatteryWidget"))
                {
                    Debug.LogFormat("<Manipulator> Object {0} deactivated.", widget.name);
                    widget.SetActive(false);

                    var newWidget = Instantiate(MysteryWidgetPrefab);
                    newWidget.transform.parent = widget.transform.parent;
                    newWidget.transform.localPosition = widget.transform.localPosition;
                    newWidget.transform.localRotation = widget.transform.localRotation;
                    newWidget.transform.localScale = widget.transform.localScale;
                    newWidget.SetActive(true);
                }
            }
        }
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

    private MethodInfo<object> GetMethod(object target, string name, int numParameters, bool isPublic = false)
    {
        if (target == null)
        {
            Debug.LogFormat("<Manipulator> Attempt to get {1} method {0} with return type void from a null object.", name, isPublic ? "public" : "non-public");
            return null;
        }
        var bindingFlags = (isPublic ? BindingFlags.Public : BindingFlags.NonPublic) | BindingFlags.Instance;
        var targetType = target.GetType();
        var mths = targetType.GetMethods(bindingFlags).Where(m => m.Name == name && m.ReturnType == typeof(void) && m.GetParameters().Length == numParameters).Take(2).ToArray();
        if (mths.Length == 0)
        {
            Debug.LogFormat("<Manipulator> Type {0} does not contain {1} method {2} with return type void and {3} parameters.", targetType, isPublic ? "public" : "non-public", name, numParameters);
            return null;
        }
        if (mths.Length > 1)
        {
            Debug.LogFormat("<Manipulator> Type {0} contains multiple {1} methods {2} with return type void and {3} parameters.", targetType, isPublic ? "public" : "non-public", name, numParameters);
            return null;
        }
        return new MethodInfo<object>(target, mths[0]);
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

    private IEnumerable<object> ProcessBackgrounds(KMBombModule module, int moduleIndex)
    {
        var comp = (MonoBehaviour) GetComponent(module, "Backgrounds");
        var fldButtonA = GetField<MeshRenderer>(comp, "ButtonAMesh", isPublic: true);
        var fldBacking = GetField<MeshRenderer>(comp, "BackingMesh", isPublic: true);
        var fldCounter = GetField<TextMesh>(comp, "CounterText", isPublic: true);
        var fldButtonATextMesh = GetField<TextMesh>(comp, "ButtonATextMesh", isPublic: true);

        if (comp == null || fldButtonA == null || fldBacking == null || fldCounter == null || fldButtonATextMesh == null)
            yield break;

        yield return null;

        fldBacking.Get().material.color = new Color(1f, 0.5f, 0f);
        fldButtonA.Get().material.color = Color.yellow;
        fldCounter.Get().text = "7";
        fldButtonATextMesh.Get().color = Color.black;
        module.HandlePass();
    }

    private IEnumerable<object> ProcessBattleship(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "BattleshipModule");
        var fldRows = GetField<TextMesh[]>(comp, "_rows");
        var fldColumns = GetField<TextMesh[]>(comp, "_columns");
        var fldShipLengthLabels = GetField<TextMesh[]>(comp, "ShipLengthLabels", isPublic: true);
        var fldSolution = GetField<bool[][]>(comp, "_solution");

        while (fldSolution.Get(nullAllowed: true) == null)
            yield return new WaitForSeconds(.1f);

        for (int i = 0; i < 5; i++)
            fldRows.Get()[i].text = "13321"[i].ToString();
        for (int i = 0; i < 5; i++)
            fldColumns.Get()[i].text = "4O3O3"[i].ToString();
        for (int i = 0; i < 4; i++)
            fldShipLengthLabels.Get()[i].text = "×1,×1,1×,1×".Split(',')[i];
    }

    private IEnumerable<object> ProcessBrushStrokes(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "BrushStrokesScript");
        var fldHorizontalStrokes = GetField<SpriteRenderer[]>(comp, "horizontalStrokes", isPublic: true);
        var fldVerticalStrokes = GetField<SpriteRenderer[]>(comp, "verticalStrokes", isPublic: true);
        var fldTlbrStrokes = GetField<SpriteRenderer[]>(comp, "tlbrStrokes", isPublic: true);
        var fldTrblStrokes = GetField<SpriteRenderer[]>(comp, "trblStrokes", isPublic: true);
        var fldBtnRenderers = GetField<MeshRenderer[]>(comp, "btnRenderers", isPublic: true);
        var fldBtnColors = GetField<Material[]>(comp, "btnColors", isPublic: true);
        var fldColorblindText = GetField<TextMesh[]>(comp, "colorblindText", isPublic: true);
        var fldBtnSelectables = GetField<KMSelectable[]>(comp, "btnSelectables", isPublic: true);

        if (comp == null || fldHorizontalStrokes == null || fldVerticalStrokes == null || fldTlbrStrokes == null || fldTrblStrokes == null || fldBtnRenderers == null || fldBtnColors == null || fldColorblindText == null || fldBtnSelectables == null)
            yield break;

        yield return null;

        var isActivated = false;
        module.OnActivate += delegate { isActivated = true; };
        while (!isActivated)
            yield return new WaitForSeconds(.1f);

        fldBtnRenderers.Get()[4].material = fldBtnColors.Get()[moduleIndex == 0 ? 6 : 3];
        fldColorblindText.Get()[4].text = "SL"[moduleIndex].ToString();

        fldBtnSelectables.Get()[4].OnInteractEnded = delegate
        {
            for (int i = 0; i < 6; i++)
            {
                fldHorizontalStrokes.Get()[i].enabled = ((moduleIndex == 0 ? 0x3F : 0x31) & (1 << i)) != 0;
                fldVerticalStrokes.Get()[i].enabled = ((moduleIndex == 0 ? 0x2D : 0x3F) & (1 << i)) != 0;
            }
            for (int i = 0; i < 4; i++)
            {
                fldTlbrStrokes.Get()[i].enabled = false;
                fldTrblStrokes.Get()[i].enabled = false;
            }
            foreach (var btn in fldBtnRenderers.Get())
                btn.material = fldBtnColors.Get()[13];

            for (int i = 0; i < 9; i++)
                fldColorblindText.Get()[i].text = "SOLVED!!!"[i].ToString();

            module.HandlePass();
        };
        fldBtnSelectables.Get()[4].OnInteract = delegate { return false; };
    }

    private IEnumerable<object> ProcessCaesarCipher(KMBombModule module, int moduleIndex)
    {
        var comp = (MonoBehaviour) GetComponent(module, "CaesarCipherModule");
        var fldButtonLabels = GetField<TextMesh[]>(comp, "ButtonLabels", isPublic: true);
        var fldDisplayText = GetField<TextMesh>(comp, "DisplayText", isPublic: true);

        if (comp == null || fldButtonLabels == null || fldDisplayText == null)
            yield break;

        while (fldDisplayText.Get().text == "")
            yield return new WaitForSeconds(.1f);

        fldDisplayText.Get().text = "AKHDS";
        for (int i = 0; i < 12; i++)
            fldButtonLabels.Get()[i].text = "ELTPOBJCIFUM"[i].ToString();
    }

    private IEnumerable<object> ProcessConnectionCheck(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "GraphModule");
        var fldL = GetField<GameObject[]>(comp, "L", isPublic: true);
        var fldR = GetField<GameObject[]>(comp, "R", isPublic: true);
        var fldLedG = GetField<GameObject[]>(comp, "LEDG", isPublic: true);
        var fldLedR = GetField<GameObject[]>(comp, "LEDR", isPublic: true);
        var fldActivated = GetField<bool>(comp, "_lightsOn");

        if (comp == null || fldL == null || fldR == null || fldLedG == null || fldLedR == null || fldActivated == null)
            yield break;

        yield return null;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(.1f);

        for (int i = 0; i < 4; i++)
        {
            //fldL.Get()[i].GetComponentInChildren<TextMesh>().text = "2167"[i].ToString();     // — for 7HPJ
            //fldR.Get()[i].GetComponentInChildren<TextMesh>().text = "3235"[i].ToString();
            fldL.Get()[i].GetComponentInChildren<TextMesh>().text = "1245"[i].ToString();     // — for 7HPJ or 8CAKE
            fldR.Get()[i].GetComponentInChildren<TextMesh>().text = "3587"[i].ToString();
        }

        for (int i = 0; i < 4; i++)
        {
            //fldLedG.Get()[i].SetActive((11 & (1 << i)) != 0);   // — for 7HPJ
            //fldLedR.Get()[i].SetActive((11 & (1 << i)) == 0);
            fldLedG.Get()[i].SetActive((9 & (1 << i)) != 0);     // — for 7HPJ or 8CAKE
            fldLedR.Get()[i].SetActive((9 & (1 << i)) == 0);
        }

        module.HandlePass();
    }

    private IEnumerable<object> ProcessCurriculum(KMBombModule module, int moduleIndex)
    {
        // NO LONGER USED IN THE PUZZLE
        var comp = GetComponent(module, "CurriculumModule");
        var fldSerial = GetField<string>(comp, "serial");
        var fldButtons = GetField<KMSelectable[]>(comp, "buttons", isPublic: true);
        var fldCells = GetField<GameObject[]>(comp, "cells", isPublic: true);

        if (comp == null || fldSerial == null || fldButtons == null || fldCells == null)
            yield break;

        while (fldSerial.Get(nullAllowed: true) == null)
            yield return new WaitForSeconds(.1f);

        var labels = "E1,L1,L1,M1,M1".Split(',');
        for (int i = 0; i < 5; i++)
            fldButtons.Get()[i].GetComponentInChildren<TextMesh>().text = labels[i];

        for (int i = 0; i < 30; i++)
        {
            fldCells.Get()[i].SetActive((0x18018798 & (1 << (29 - i))) != 0);
            fldCells.Get()[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }

        module.HandlePass();
    }

    private IEnumerable<object> ProcessManometers(KMBombModule module, int moduleIndex)
    {
        var comp = (MonoBehaviour) GetComponent(module, "manometers");
        var fldActivated = GetField<bool>(comp, "_lightsOn");
        var fldScreen = GetField<MeshRenderer>(comp, "pScreen", isPublic: true);
        var fldMinus = GetField<MeshRenderer>(comp, "minus", isPublic: true);
        var fldPlus = GetField<MeshRenderer>(comp, "plus", isPublic: true);
        var fldScreenText = GetField<TextMesh>(comp, "screenT", isPublic: true);
        var fldTextT = GetField<TextMesh>(comp, "T", isPublic: true);
        var fldTextBR = GetField<TextMesh>(comp, "BL", isPublic: true);
        var fldTextBL = GetField<TextMesh>(comp, "BR", isPublic: true);

        if (comp == null || fldActivated == null || fldMinus == null || fldPlus == null || fldScreenText == null || fldTextT == null || fldTextBL == null || fldTextBR == null)
            yield break;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(.1f);

        // Stop the screen from blinking
        comp.StopCoroutine("blink");

        // Set up screen/+/- button colors
        fldScreen.Get().material.color = Color.black;
        fldMinus.Get().material.color = Color.yellow;
        fldPlus.Get().material.color = Color.blue;
        fldScreenText.Get().text = "32";

        // Set up the manometers
        fldTextT.Get().text = "10";
        fldTextBR.Get().text = "8";
        fldTextBL.Get().text = "10";

        fldTextT.Get().color = new Color32(226, 43, 11, byte.MaxValue);
        fldTextBL.Get().color = new Color32(226, 43, 11, byte.MaxValue);
        fldTextBR.Get().color = new Color32(226, 43, 11, byte.MaxValue);

        module.HandlePass();
    }

    private IEnumerable<object> ProcessMicrocontroller(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "Micro");
        var fldBackground = GetField<GameObject>(comp, "Background", isPublic: true);
        var fldMicBig = GetField<GameObject>(comp, "MicBig", isPublic: true);
        var fldMicMed = GetField<GameObject>(comp, "MicMed", isPublic: true);
        var fldMicSmall = GetField<GameObject>(comp, "MicSmall", isPublic: true);
        var fldBg3 = GetField<Material>(comp, "BG3", isPublic: true);
        var fldLeds = GetField<GameObject[]>(comp, "LEDS", isPublic: true);
        var fldDot = GetField<GameObject>(comp, "Dot", isPublic: true);
        var fldLedMaterials = GetField<Material[]>(comp, "LEDMaterials", isPublic: true);
        var fldMicSerial = GetField<TextMesh>(comp, "MicSerial", isPublic: true);
        var fldMicType = GetField<TextMesh>(comp, "MicType", isPublic: true);

        if (comp == null || fldBackground == null || fldMicBig == null || fldMicMed == null || fldMicSmall == null || fldBg3 == null || fldLeds == null || fldDot == null || fldLedMaterials == null || fldMicSerial == null || fldMicType == null)
            yield break;

        var activated = false;
        module.OnActivate += delegate { activated = true; };
        while (!activated)
            yield return new WaitForSeconds(.1f);

        var background = fldBackground.Get();
        var micBig = fldMicBig.Get();
        var micMed = fldMicMed.Get();
        var micSmall = fldMicSmall.Get();
        var bg3 = fldBg3.Get();
        var leds = fldLeds.Get();
        var dot = fldDot.Get();
        var ledMaterials = fldLedMaterials.Get();

        background.GetComponent<Renderer>().material = bg3;
        micBig.GetComponent<Renderer>().enabled = false;
        micMed.GetComponent<Renderer>().enabled = false;
        micSmall.GetComponent<Renderer>().enabled = true;
        leds[6].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().enabled = false;
        leds[6].transform.Find("Plane").gameObject.GetComponent<Renderer>().enabled = false;
        leds[7].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().enabled = false;
        leds[7].transform.Find("Plane").gameObject.GetComponent<Renderer>().enabled = false;
        leds[8].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().enabled = false;
        leds[8].transform.Find("Plane").gameObject.GetComponent<Renderer>().enabled = false;
        leds[9].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().enabled = false;
        leds[9].transform.Find("Plane").gameObject.GetComponent<Renderer>().enabled = false;
        dot.transform.localPosition = new Vector3(-0.0004618395f, 0.002604042f, 0.001104411f);

        leds[0].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[5]; // blue
        leds[2].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[6]; // green
        leds[4].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[4]; // magenta
        leds[5].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[2]; // red
        leds[3].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[3]; // yellow
        leds[1].transform.Find("Plane.001").gameObject.GetComponent<Renderer>().material = ledMaterials[1]; // white

        fldMicType.Get().text = "EXPL";
        fldMicSerial.Get().text = "FNX 562-3";
        module.HandlePass();
    }

    private IEnumerable<object> ProcessMurder(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "MurderModule");
        var fldActivated = GetField<bool>(comp, "isActivated");
        var fldDisplays = GetField<TextMesh[]>(comp, "Display", isPublic: true);

        if (comp == null || fldDisplays == null || fldActivated == null)
            yield break;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(.1f);

        fldDisplays.Get()[0].text = "Mrs Peacock";
        fldDisplays.Get()[0].color = new Color(0.3f, 0.3f, 1f);
        fldDisplays.Get()[1].text = "Candlestick";
        fldDisplays.Get()[2].text = "Library";
        module.HandlePass();
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

    private IEnumerable<object> ProcessOnlyConnect(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "OnlyConnectModule");
        var fldHieroglyphs = GetField<Material[]>(comp, "EgyptianHieroglyphs", isPublic: true);
        var fldButtons = GetField<KMSelectable[]>(comp, "EgyptianHieroglyphButtons", isPublic: true);
        var fldTeamName = GetField<TextMesh>(comp, "TeamName", isPublic: true);

        if (comp == null || fldHieroglyphs == null || fldButtons == null || fldTeamName == null)
            yield break;

        yield return null;

        var isActivated = false;
        module.OnActivate += delegate { isActivated = true; };
        while (!isActivated)
            yield return new WaitForSeconds(.1f);

        // Arrangement: 2R, TF, Ey, HV, Wa, Li
        fldButtons.Get()[0].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[0];
        fldButtons.Get()[1].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[2];
        fldButtons.Get()[2].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[5];
        fldButtons.Get()[3].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[3];
        fldButtons.Get()[4].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[4];
        fldButtons.Get()[5].GetComponent<MeshRenderer>().material = fldHieroglyphs.Get()[1];
        fldTeamName.Get().text = "CORPUSCLES";
    }

    private IEnumerable<object> ProcessPerspectivePegs(KMBombModule module, int moduleIndex)
    {
        // OBSOLETE — no longer used in the puzzle

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

    private IEnumerable<object> ProcessPointOfOrder(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "PointOfOrderModule");
        var fldCardImages = GetField<Texture[]>(comp, "CardImages", isPublic: true);

        if (comp == null || fldCardImages == null)
            yield break;

        var isActivated = false;
        module.OnActivate += delegate { isActivated = true; };
        while (!isActivated)
            yield return new WaitForSeconds(.1f);

        var cardImages = fldCardImages.Get();

        // A♠ 6♠ 2♣ 6♦ 10♣
        module.transform.Find("PileCard1").GetComponent<MeshRenderer>().material.mainTexture = cardImages.First(ci => ci.name == "Ace of Spades");
        module.transform.Find("PileCard2").GetComponent<MeshRenderer>().material.mainTexture = cardImages.First(ci => ci.name == "Six of Spades");
        module.transform.Find("PileCard3").GetComponent<MeshRenderer>().material.mainTexture = cardImages.First(ci => ci.name == "Two of Clubs");
        module.transform.Find("PileCard4").GetComponent<MeshRenderer>().material.mainTexture = cardImages.First(ci => ci.name == "Six of Diamonds");
        module.transform.Find("PileCard5").GetComponent<MeshRenderer>().material.mainTexture = cardImages.First(ci => ci.name == "Ten of Clubs");
    }

    private IEnumerable<object> ProcessResistors(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "ResistorsModule");
        var mthDisplay = GetMethod(comp, "DisplayResistor", 7);
        var fldBands = GetField<MeshRenderer[]>(comp, "bands", isPublic: true);

        if (comp == null || mthDisplay == null||fldBands == null )
            yield break;

        var activated = false;
        module.OnActivate += delegate { activated = true; };
        while (!activated)
            yield return new WaitForSeconds(.1f);

        foreach (var i in new[] { 1, 3, 6, 8 })
            fldBands.Get()[i].enabled = true;
        mthDisplay.Invoke(66000d, 0, 1, 2, 3, 4, "");
        mthDisplay.Invoke(330000d, 5, 6, 7, 8, 9, "");
    }

    private IEnumerable<object> ProcessRockPaperScissorsLizardSpock(KMBombModule module, int moduleIndex)
    {
        // OBSOLETE — no longer used in the puzzle (has been replaced with Only Connect)

        var comp = GetComponent(module, "RockPaperScissorsLizardSpockModule");
        var fldRock = GetField<Transform>(comp, "Rock", isPublic: true);
        var fldPaper = GetField<Transform>(comp, "Paper", isPublic: true);
        var fldScissors = GetField<Transform>(comp, "Scissors", isPublic: true);
        var fldLizard = GetField<Transform>(comp, "Lizard", isPublic: true);
        var fldSpock = GetField<Transform>(comp, "Spock", isPublic: true);
        var fldMatCorrect = GetField<Material>(comp, "MatCorrect", isPublic: true);

        yield return null;

        fldRock.Get().localPosition = new Vector3(0.02830811f, .01506f, 0.04099593f);
        fldPaper.Get().localPosition = new Vector3(-0.024f, .01506f, 0.079f);
        fldScissors.Get().localPosition = new Vector3(0.008328188f, .01506f, -0.02049594f);
        fldLizard.Get().localPosition = new Vector3(-0.07630811f, .01506f, 0.04099593f);
        fldSpock.Get().localPosition = new Vector3(-0.05632819f, .01506f, -0.02049594f);

        fldRock.Get().GetComponent<MeshRenderer>().material = fldMatCorrect.Get();
        fldSpock.Get().GetComponent<MeshRenderer>().material = fldMatCorrect.Get();
        module.HandlePass();
    }

    private IEnumerable<object> ProcessScrew(KMBombModule module, int moduleIndex)
    {
        var comp = (MonoBehaviour) GetComponent(module, "Screw");
        var fldActivated = GetField<bool>(comp, "_lightsOn");
        var fldScreenText = GetField<TextMesh>(comp, "screenText", isPublic: true);

        if (comp == null || fldActivated == null || fldScreenText == null)
            yield break;

        while (!fldActivated.Get())
            yield return new WaitForSeconds(.1f);

        comp.StartCoroutine("ScrewOut");
        yield return new WaitForSeconds(1f);
        GetField<int>(comp, "screwLoc").Set(5);
        comp.StartCoroutine("ScrewIn");
        fldScreenText.Get().text = "";
        module.HandlePass();
    }

    private IEnumerable<object> ProcessScripting(KMBombModule module, int moduleIndex)
    {
        var comp = GetComponent(module, "KritScript");
        var fldUsingProgram1 = GetField<TextMesh>(comp, "UsingProgram1", isPublic: true);
        var fldUsingProgram2 = GetField<TextMesh>(comp, "UsingProgram2", isPublic: true);
        var fldUsingProgram3 = GetField<TextMesh>(comp, "UsingProgram3", isPublic: true);
        var fldUsingProgram1Value = GetField<string>(comp, "UsingProgram1Value");
        var fldUsingProgram2Value = GetField<string>(comp, "UsingProgram2Value");
        var fldUsingProgram3Value = GetField<string>(comp, "UsingProgram3Value");
        var fldUsing1 = GetField<TextMesh>(comp, "Using1", isPublic: true);
        var fldUsing2 = GetField<TextMesh>(comp, "Using2", isPublic: true);
        var fldUsing3 = GetField<TextMesh>(comp, "Using3", isPublic: true);
        var fldVariableObj = GetField<GameObject>(comp, "VariableObj", isPublic: true);
        var fldVariable = GetField<TextMesh>(comp, "Variable", isPublic: true);
        var fldValueObj = GetField<GameObject>(comp, "ValueObj", isPublic: true);
        var fldVarName = GetField<TextMesh>(comp, "VarName", isPublic: true);
        var fldCondition = GetField<TextMesh>(comp, "Condition", isPublic: true);
        var fldVariableTxtMsh = GetField<TextMesh>(comp, "VariableTxtMsh", isPublic: true);
        var fldVariableValueTxtMsh = GetField<TextMesh>(comp, "VariableValueTxtMsh", isPublic: true);
        var fldBoolVarPosition = GetField<Vector3>(comp, "BoolVarPosition");
        var fldActionTxtMsh = GetField<TextMesh>(comp, "ActionTxtMsh", isPublic: true);
        var fldScriptLines = GetField<GameObject>(comp, "ScriptLines", isPublic: true);
        var fldStatusCorrect = GetField<GameObject>(comp, "StatusCorrect", isPublic: true);
        var fldRunBtn = GetField<KMSelectable>(comp, "RunBtn", isPublic: true);

        if (comp == null || fldUsingProgram1 == null || fldUsingProgram2 == null || fldUsingProgram3 == null || fldUsing1 == null || fldUsing2 == null || fldUsing3 == null ||
                fldVariableObj == null || fldVariableValueTxtMsh == null || fldBoolVarPosition == null || fldActionTxtMsh == null || fldScriptLines == null || fldStatusCorrect == null || fldRunBtn == null)
            yield break;

        yield return null;

        fldUsingProgram1.Get().color = new Color32(173, 173, 173, 255);
        fldUsingProgram2.Get().color = new Color32(173, 173, 173, 125);
        fldUsingProgram3.Get().color = new Color32(173, 173, 173, 255);
        fldUsing1.Get().color = new Color32(34, 123, 156, 255);
        fldUsing2.Get().color = new Color32(34, 123, 156, 155);
        fldUsing3.Get().color = new Color32(34, 123, 156, 255);

        fldVariable.Get().text = "bool";
        fldVarName.Get().text = "IsLit;";
        fldCondition.Get().text = "(IsLit)";
        fldVariableTxtMsh.Get().text = "IsLit = ";
        fldVariableObj.Get().transform.localPosition = fldBoolVarPosition.Get();
        fldVariableValueTxtMsh.Get().text = "true<color=#ADADAD>;</color>";
        fldVariableValueTxtMsh.Get().color = new Color32(34, 123, 156, 255);
        fldValueObj.Get().transform.localPosition = new Vector3(0.2514f, 0.5100001f, -0.09799998f);

        var hasKtane = fldUsingProgram1Value.Get() == "KTaNE" || fldUsingProgram2Value.Get() == "KTaNE" || fldUsingProgram3Value.Get() == "KTaNE";
        fldActionTxtMsh.Get().text = hasKtane ? "HandleSolve();" : "HandleStrike();";

        fldRunBtn.Get().OnInteract = delegate
        {
            fldScriptLines.Get().SetActive(false);
            fldStatusCorrect.Get().SetActive(true);
            module.HandlePass();
            return false;
        };
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
    }
}
