using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LFE
{
    public class HeightMeasurePlugin : MVRScript
    {
        private static Color FACE_MARKER_COLOR = Color.blue;

        public DAZSkinV2 Skin;

        readonly IVertexPosition[] _verticesBust = new IVertexPosition[] {
            new VertexPositionMiddle(7213, 17920), // midchest 1/2 way between the nipples at bust height
            new VertexPositionExact(17920), // bust -- right nipple just to the left
            new VertexPositionExact(10939), // bust -- right nipple just to the right
            new VertexPositionExact(19588),
            new VertexPositionExact(19617),
            new VertexPositionExact(13233),
            new VertexPositionExact(11022), // bust -- right back
            new VertexPositionExact(10495), // bust -- back center
        };

        readonly IVertexPosition[] _verticesUnderbust = new IVertexPosition[] {
            new VertexPositionMiddle(10822, 10820), // mid chest
            new VertexPositionExact(21469), // right breast under nipple
            new VertexPositionExact(21470), // right breast under nipple
            new VertexPositionExact(21394), // right side 
            new VertexPositionMiddle(11022, 21508, 0.4f),
            new VertexPositionExact(2100), // back
        };

        readonly IVertexPosition[] _verticesWaist = new IVertexPosition[] {
            new VertexPositionExact(8152), // front and center
            new VertexPositionExact(19663), // front right 1
            new VertexPositionExact(13675), // front right 2
            new VertexPositionExact(13715), // front right 3
            new VertexPositionExact(13727), // right side
            new VertexPositionExact(13725), // back curve 1
            new VertexPositionExact(2921), // back
        };

        readonly IVertexPosition[] _verticesHip = new IVertexPosition[] {
            new VertexPositionExact(22843), // front and center
            new VertexPositionExact(13750), // front right 1
            new VertexPositionExact(18460), // front right 2
            new VertexPositionMiddle(11234, 18491, 0.8f), // front right 3
            new VertexPositionExact(18512), // right side
            new VertexPositionExact(18529), // glute curve 1
            new VertexPositionExact(18562), // glute curve 2
            new VertexPositionMiddle(18562, 7878), // glute middle
        };

        readonly IVertexPosition _vertexHead = new VertexPositionExact(2087);
        readonly IVertexPosition _vertexChin = new VertexPositionExact(2079);
        readonly IVertexPosition _vertexEarLeft = new VertexPositionExact(3236);
        readonly IVertexPosition _vertexEarRight = new VertexPositionExact(20646);
        readonly IVertexPosition _vertexUnderbust = new VertexPositionExact(21469); // right breast under nipple
        readonly IVertexPosition _vertexNipple = new VertexPositionExact(10939);
        readonly IVertexPosition _vertexNavel = new VertexPositionMiddle(18824, 8147);
        readonly IVertexPosition _vertexGroin = new VertexPositionExact(22208);
        readonly IVertexPosition _vertexKnee = new VertexPositionMiddle(8508, 19179);
        readonly IVertexPosition _vertexShoulder = new VertexPositionMiddle(11110, 182);
        readonly IVertexPosition _vertexEyeLeftTopHeight = new VertexPositionMiddle(7478, 1930);
        // readonly IVertexPosition _vertexEyeLeftMidHeight = new VertexPositionMiddle(14006, 18050, 0.1f);
        readonly IVertexPosition _vertexEyeLeftMidHeight = new VertexPositionExact(14006);
        readonly IVertexPosition _vertexEyeLeftInner = new VertexPositionExact(7575); // a little past the lacrimal -- this is actually part of the nose
        readonly IVertexPosition _vertexEyeLeftBottom = new VertexPositionExact(3187);
        readonly IVertexPosition _vertexEyeLeftOuterHeight = new VertexPositionExact(7351);
        readonly IVertexPosition _vertexEyeRightTopHeight = new VertexPositionMiddle(18175, 12858);
        readonly IVertexPosition _vertexEyeRightMidHeight = new VertexPositionMiddle(3223, 7351, 0.2f);
        readonly IVertexPosition _vertexEyeRightInner = new VertexPositionExact(18267); // a little past the lacrimal -- this is actually part of the nose
        readonly IVertexPosition _vertexEyeRightBottom = new VertexPositionExact(13972);
        readonly IVertexPosition _vertexEyeRightOuter = new VertexPositionExact(18050); 
        readonly IVertexPosition _vertexNoseTip = new VertexPositionExact(2111);
        readonly IVertexPosition _vertexNoseBottom = new VertexPositionExact(3252);
        readonly IVertexPosition _vertexMouthLeftSideMiddle = new VertexPositionExact(1655);
        readonly IVertexPosition _vertexMouthRightSideMiddle = new VertexPositionExact(12319);
        readonly IVertexPosition _vertexMouthMidHeight = new VertexPositionMiddle(2136, 2145);

        DAZBone _eyeLeft;


        // measurement storables
        JSONStorableFloat _fullHeightStorable;
        JSONStorableFloat _headSizeHeightStorable;
        JSONStorableFloat _headSizeWidthStorable;
        JSONStorableFloat _chinHeightStorable;
        JSONStorableFloat _shoulderHeightStorable;
        JSONStorableFloat _nippleHeightStorable;
        JSONStorableFloat _underbustHeightStorable;
        JSONStorableFloat _navelHeightStorable;
        JSONStorableFloat _crotchHeightStorable;
        JSONStorableFloat _kneeBottomHeightStorable;
        JSONStorableFloat _heightInHeadsStorable;
        JSONStorableFloat _breastBustStorable;
        JSONStorableFloat _breastUnderbustStorable;
        JSONStorableFloat _breastBandStorable;
        JSONStorableString _breastCupStorable;
        JSONStorableFloat _waistSizeStorable;
        JSONStorableFloat _hipSizeStorable;

        // TOOD: shoulder width

        // other storables
        JSONStorableFloat _markerSpreadStorable;
        JSONStorableFloat _markerLeftRightStorable;
        JSONStorableFloat _markerFrontBackStorable;
        JSONStorableBool _showTapeMarkersStorable;
        JSONStorableBool _showHeadHeightMarkersStorable;
        JSONStorableBool _showFeatureMarkersStorable;
        JSONStorableBool _showFeatureMarkerLabelsStorable;
        JSONStorableBool _showFaceMarkersStorable;
        JSONStorableStringChooser _cupAlgorithmStorable;
        JSONStorableFloat _lineThicknessStorable;

        DAZCharacter _dazCharacter;

        readonly ICupCalculator[] _cupCalculators = new ICupCalculator[] {
            new SizeChartCupCalculator(),
            new KnixComCupCalculator(),
            new ChateLaineCupCalculator()
        };

        HorizontalMarker _markerHead;
        HorizontalMarker _markerChin;
        HorizontalMarker _markerShoulder;
        HorizontalMarker _markerNipple;
        HorizontalMarker _markerUnderbust;
        HorizontalMarker _markerNavel;
        HorizontalMarker _markerGroin;
        HorizontalMarker _markerKnee;
        HorizontalMarker _markerHeel;

        HorizontalMarker _markerEyeMidHeight;
        HorizontalMarker _markerEyeRightOuter;
        HorizontalMarker _markerEyeLeftOuter;
        HorizontalMarker _markerMouthMidHeight;
        HorizontalMarker _markerMouthLeft;
        HorizontalMarker _markerMouthRight;
        HorizontalMarker _markerNoseBottomHeight;
        HorizontalMarker _markerChinSmall;
        HorizontalMarker _markerHeadSmall;
        HorizontalMarker _markerHeadLeft;
        HorizontalMarker _markerHeadRight;
        HorizontalMarker _markerFaceCenter;


        public void InitStorables() {
            // Cup algorithm choice
            _cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );

            // Float: How far apart markers are spread apart
            _markerSpreadStorable = new JSONStorableFloat("Spread Markers", 0.02f, -1, 1);
            RegisterFloat(_markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            _markerFrontBackStorable = new JSONStorableFloat("Move Markers Forward/Backward", 0.15f, -5, 5);
            RegisterFloat(_markerFrontBackStorable);

            // Float: Move markers left or right 
            _markerLeftRightStorable = new JSONStorableFloat("Move Markers Left/Right", 0, -5, 5);
            RegisterFloat(_markerLeftRightStorable);

            // Bool: Show the tape measures for circumference?
            _showTapeMarkersStorable = new JSONStorableBool("Show Tape Measure Markers", false);
            RegisterBool(_showTapeMarkersStorable);

            // Bool: Show head hight markers (white ones)
            _showHeadHeightMarkersStorable = new JSONStorableBool("Show Head Height Markers", true);
            RegisterBool(_showHeadHeightMarkersStorable);

            // Bool: Show the main feature markers for the figure
            _showFeatureMarkersStorable = new JSONStorableBool("Show Figure Feature Markers", true);
            RegisterBool(_showFeatureMarkersStorable);

            // Bool: Show the feature marker labels
            _showFeatureMarkerLabelsStorable = new JSONStorableBool("Show Feature Marker Labels", true);
            RegisterBool(_showFeatureMarkerLabelsStorable);

            // Bool: Show face markers
            _showFaceMarkersStorable = new JSONStorableBool("Show Face Markers", true);
            RegisterBool(_showFeatureMarkerLabelsStorable);

            // Float: Line thickness
            _lineThicknessStorable = new JSONStorableFloat("Line Thickness", 2, 1, 10, constrain: true);
            RegisterFloat(_lineThicknessStorable);


            // calculated positions and distances for other plugins to use if wanted
            _fullHeightStorable = new JSONStorableFloat("figureHeight", 0, 0, 100);
            RegisterFloat(_fullHeightStorable);

            _heightInHeadsStorable = new JSONStorableFloat("figureHeightHeads", 0, 0, 100);
            RegisterFloat(_heightInHeadsStorable);

            _headSizeHeightStorable = new JSONStorableFloat("headSizeHeight", 0, 0, 100);
            RegisterFloat(_headSizeHeightStorable);

            _chinHeightStorable = new JSONStorableFloat("chinHeight", 0, 0, 100);
            RegisterFloat(_chinHeightStorable);

            _shoulderHeightStorable = new JSONStorableFloat("shoulderHeight", 0, 0, 100);
            RegisterFloat(_shoulderHeightStorable);

            _nippleHeightStorable = new JSONStorableFloat("nippleHeight", 0, 0, 100);
            RegisterFloat(_nippleHeightStorable);

            _underbustHeightStorable = new JSONStorableFloat("underbustHeight", 0, 0, 100);
            RegisterFloat(_underbustHeightStorable);

            _navelHeightStorable = new JSONStorableFloat("navelHeight", 0, 0, 100);
            RegisterFloat(_navelHeightStorable);

            _crotchHeightStorable = new JSONStorableFloat("crotchHeight", 0, 0, 100);
            RegisterFloat(_crotchHeightStorable);

            _kneeBottomHeightStorable = new JSONStorableFloat("kneeHeight", 0, 0, 100);
            RegisterFloat(_kneeBottomHeightStorable);

            _headSizeWidthStorable = new JSONStorableFloat("headSizeWidth", 0, 0, 100);
            RegisterFloat(_headSizeWidthStorable);

            _breastBustStorable = new JSONStorableFloat("breastBustSize", 0, 0, 100);
            RegisterFloat(_breastBustStorable);

            _breastUnderbustStorable = new JSONStorableFloat("breastUnderbustSize", 0, 0, 100);
            RegisterFloat(_breastUnderbustStorable);

            _breastBandStorable = new JSONStorableFloat("breastBandSize", 0, 0, 100);
            RegisterFloat(_breastBandStorable);

            _breastCupStorable = new JSONStorableString("breastCupSize", "");
            RegisterString(_breastCupStorable);

            _waistSizeStorable = new JSONStorableFloat("waistSize", 0, 0, 100);
            RegisterFloat(_waistSizeStorable);

            _hipSizeStorable = new JSONStorableFloat("hipSize", 0, 0, 100);
            RegisterFloat(_hipSizeStorable);

        }

        public override void Init()
        {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            // initialize the line markers
            InitLineMarkers();

            // initialize storables
            InitStorables();

            // initialize the ui components
            CreateScrollablePopup(_cupAlgorithmStorable);
            CreateSlider(_lineThicknessStorable);
            CreateSlider(_markerSpreadStorable, rightSide: true);
            CreateSlider(_markerFrontBackStorable, rightSide: true);
            CreateSlider(_markerLeftRightStorable, rightSide: true);
            CreateToggle(_showTapeMarkersStorable);
            CreateToggle(_showHeadHeightMarkersStorable);
            CreateToggle(_showFeatureMarkersStorable);
            CreateToggle(_showFeatureMarkerLabelsStorable);
            CreateToggle(_showFaceMarkersStorable);
        }

        public void InitLineMarkers() {
            // this does not position them, just creates them
            _markerHead = gameObject.AddComponent<HorizontalMarker>();
            _markerHead.Name = "Head";
            _markerHead.Color = Color.green;

            _markerChin = gameObject.AddComponent<HorizontalMarker>();
            _markerChin.Name = "Chin";
            _markerChin.Color = Color.green;

            _markerShoulder = gameObject.AddComponent<HorizontalMarker>();
            _markerShoulder.Name = "Shoulder";
            _markerShoulder.Color = Color.green;

            _markerNipple = gameObject.AddComponent<HorizontalMarker>();
            _markerNipple.Name = "Nipple";
            _markerNipple.Color = Color.green;

            _markerUnderbust = gameObject.AddComponent<HorizontalMarker>();
            _markerUnderbust.Name = "Underbust";
            _markerUnderbust.Color = Color.green;

            _markerNavel = gameObject.AddComponent<HorizontalMarker>();
            _markerNavel.Name = "Navel";
            _markerNavel.Color = Color.green;

            _markerGroin = gameObject.AddComponent<HorizontalMarker>();
            _markerGroin.Name = "Groin";
            _markerGroin.Color = Color.green;

            _markerKnee = gameObject.AddComponent<HorizontalMarker>();
            _markerKnee.Name = "Knee";
            _markerKnee.Color = Color.green;

            _markerHeel = gameObject.AddComponent<HorizontalMarker>();
            _markerHeel.Name = "Heel";
            _markerHeel.Color = Color.green;

            _eyeLeft = containingAtom.GetStorableByID("lEye") as DAZBone;

            _markerEyeMidHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeMidHeight.Name = "Eye Height";
            _markerEyeMidHeight.Color = FACE_MARKER_COLOR;

            _markerEyeRightOuter = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeRightOuter.Name = "Eye Right Outer";
            _markerEyeRightOuter.Color = FACE_MARKER_COLOR;
            _markerEyeRightOuter.LineDirection = Vector3.up;

            _markerEyeLeftOuter = gameObject.AddComponent<HorizontalMarker>();
            _markerEyeLeftOuter.Name = "Eye Left Outer";
            _markerEyeLeftOuter.Color = FACE_MARKER_COLOR;
            _markerEyeLeftOuter.LineDirection = Vector3.up;

            _markerNoseBottomHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerNoseBottomHeight.Name = "Nose Bottom Height";
            _markerNoseBottomHeight.Color = FACE_MARKER_COLOR;

            _markerMouthMidHeight = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthMidHeight.Name = "Mouth Height";
            _markerMouthMidHeight.Color = FACE_MARKER_COLOR;

            _markerMouthLeft = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthLeft.Name = "Mouth Left";
            _markerMouthLeft.Color = FACE_MARKER_COLOR;
            _markerMouthLeft.LineDirection = Vector3.up;

            _markerMouthRight = gameObject.AddComponent<HorizontalMarker>();
            _markerMouthRight.Name = "Mouth Right";
            _markerMouthRight.Color = FACE_MARKER_COLOR;
            _markerMouthRight.LineDirection = Vector3.up;

            _markerChinSmall = gameObject.AddComponent<HorizontalMarker>();
            _markerChinSmall.Name = "Chin Small";
            _markerChinSmall.Color = FACE_MARKER_COLOR;

            _markerHeadSmall = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadSmall.Name = "Head Small";
            _markerHeadSmall.Color = FACE_MARKER_COLOR;

            _markerHeadRight = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadRight.Name = "Head Right";
            _markerHeadRight.Color = FACE_MARKER_COLOR;
            _markerHeadRight.LineDirection = Vector2.up;

            _markerHeadLeft = gameObject.AddComponent<HorizontalMarker>();
            _markerHeadLeft.Name = "Head Left";
            _markerHeadLeft.Color = FACE_MARKER_COLOR;
            _markerHeadLeft.LineDirection = Vector3.up;

            _markerFaceCenter = gameObject.AddComponent<HorizontalMarker>();
            _markerFaceCenter.Name = "Face Center";
            _markerFaceCenter.Color = FACE_MARKER_COLOR;
            _markerFaceCenter.LineDirection = Vector3.up;

        }

        public void OnDestroy() {
            // destroy the markers
            foreach(var m in gameObject.GetComponentsInChildren<HorizontalMarker>()) {
                Destroy(m);
            }

            foreach(var h in _bustMarkersFromMorph) {
                Destroy(h);
            }
            _bustMarkersFromMorph = new List<GameObject>();

            foreach(var h in _underbustMarkersFromMorph) {
                Destroy(h);
            }
            _underbustMarkersFromMorph = new List<GameObject>();

            foreach(var h in _waistMarkersFromMorph) {
                Destroy(h);
            }
            _waistMarkersFromMorph = new List<GameObject>();

            foreach(var h in _hipMarkersFromMorph) {
                Destroy(h);
            }
            _hipMarkersFromMorph = new List<GameObject>();
        }

        private readonly float _updateEverySeconds = 0.05f;
        private float _updateCountdown = 0;
        public void Update() {
            _dazCharacter = containingAtom.GetComponentInChildren<DAZCharacter>();
            Skin = _dazCharacter.skin;

            if(SuperController.singleton.freezeAnimation) {
                return;
            }

            // throttle the update loop
            _updateCountdown -= Time.deltaTime;
            if(_updateCountdown > 0) {
                return;
            }
            _updateCountdown = _updateEverySeconds;


            try {
                UpdateMeasurements();
                UpdateMarkerPositions();
                UpdateMarkerLabels();
            }
            catch(Exception e) {
                SuperController.LogError(e.ToString());
            }
        }

        private void UpdateMarkerPositions() {
            Vector3 pos;
            // head
            pos = _vertexHead.Position(this);
            pos.x -= _markerSpreadStorable.val + _markerLeftRightStorable.val;
            pos.z += _markerFrontBackStorable.val;
            _markerHead.Origin = pos;
            _markerHead.Enabled = _showFeatureMarkersStorable.val;
            _markerHead.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerHead.Thickness = _lineThicknessStorable.val * 0.001f;

            // chin
            pos = _vertexChin.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerChin.Origin = pos;
            _markerChin.Enabled = _showFeatureMarkersStorable.val;
            _markerChin.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerChin.Thickness = _lineThicknessStorable.val * 0.001f;

            // shoulder
            pos = _vertexShoulder.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerShoulder.Origin = pos;
            _markerShoulder.Enabled = _showFeatureMarkersStorable.val;
            _markerShoulder.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerShoulder.Thickness = _lineThicknessStorable.val * 0.001f;

            // nipple
            pos = _vertexNipple.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerNipple.Origin = pos;
            _markerNipple.Enabled = _dazCharacter.isMale ? false : _showFeatureMarkersStorable.val; // TODO: better male nipple detection
            _markerNipple.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerNipple.Thickness = _lineThicknessStorable.val * 0.001f;

            // underbust
            pos = _vertexUnderbust.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerUnderbust.Origin = pos;
            _markerUnderbust.Enabled = _dazCharacter.isMale ? false : _showFeatureMarkersStorable.val;
            _markerUnderbust.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerUnderbust.Thickness = _lineThicknessStorable.val * 0.001f;

            // navel
            pos = _vertexNavel.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerNavel.Origin = pos;
            _markerNavel.Enabled = _showFeatureMarkersStorable.val;
            _markerNavel.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerNavel.Thickness = _lineThicknessStorable.val * 0.001f;

            // groin
            pos = _vertexGroin.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerGroin.Origin = pos;
            _markerGroin.Enabled = _showFeatureMarkersStorable.val;
            _markerGroin.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerGroin.Thickness = _lineThicknessStorable.val * 0.001f;

            // knee
            pos = _vertexKnee.Position(this);
            pos.x = _markerHead.Origin.x;
            pos.z = _markerHead.Origin.z;
            _markerKnee.Origin = pos;
            _markerKnee.Enabled = _showFeatureMarkersStorable.val;
            _markerKnee.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerKnee.Thickness = _lineThicknessStorable.val * 0.001f;

            // heel - can not find vertex for heel - using colliders
            var rFoot = containingAtom.GetComponentsInChildren<CapsuleCollider>().FirstOrDefault(c => ColliderName(c).Equals("rFoot/_Collider1")); 
            if(rFoot) {
                pos = rFoot.transform.position;
                pos.x = _markerHead.Origin.x;
                pos.z = _markerHead.Origin.z;
                _markerHeel.Origin = pos;
                _markerHeel.Enabled = _showFeatureMarkersStorable.val;
                _markerHeel.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
                _markerHeel.Thickness = _lineThicknessStorable.val * 0.001f;
            }
            
            UpdateHeadHeightMarkers();

            if(!_dazCharacter.isMale) {
                UpdateBustMarkersFromMorphVertex();
                UpdateUnderbustMarkersFromMorphVertex();
            }
            UpdateWaistMarkersFromMorphVertex();
            UpdateHipMarkersFromMorphVertex();

            var midpoint = _vertexHead.Position(this);
            var midpointX = midpoint.x + (_headSizeWidthStorable.val / 2) - _markerLeftRightStorable.val;

            // eye midline
            pos = _eyeLeft.transform.position; // comes from a daz bone not a vertex
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeMidHeight.Length = _headSizeWidthStorable.val;
            _markerEyeMidHeight.Origin = pos;
            _markerEyeMidHeight.Enabled = _showFaceMarkersStorable.val;
            _markerEyeMidHeight.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerEyeMidHeight.Thickness = _lineThicknessStorable.val * 0.001f;

            // eye right outer
            pos = _vertexEyeRightOuter.Position(this);
            pos.x = pos.x - _markerLeftRightStorable.val;
            pos.y = _markerEyeMidHeight.Origin.y - _headSizeHeightStorable.val / 4 / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeRightOuter.Length = _headSizeHeightStorable.val / 4;
            _markerEyeRightOuter.Origin = pos;
            _markerEyeRightOuter.Enabled = _showFaceMarkersStorable.val;
            _markerEyeRightOuter.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerEyeRightOuter.Thickness = _lineThicknessStorable.val * 0.001f;

            // eye left outer
            pos = _vertexEyeLeftOuterHeight.Position(this);
            pos.x = pos.x - _markerLeftRightStorable.val;
            pos.y = _markerEyeMidHeight.Origin.y - _headSizeHeightStorable.val / 4 / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerEyeLeftOuter.Length = _headSizeHeightStorable.val / 4;
            _markerEyeLeftOuter.Origin = pos;
            _markerEyeLeftOuter.Enabled = _showFaceMarkersStorable.val;
            _markerEyeLeftOuter.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerEyeLeftOuter.Thickness = _lineThicknessStorable.val * 0.001f;

            // nose bottom
            pos = _vertexNoseBottom.Position(this);
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerNoseBottomHeight.Length = _headSizeWidthStorable.val;
            _markerNoseBottomHeight.Origin = pos;
            _markerNoseBottomHeight.Enabled = _showFaceMarkersStorable.val;
            _markerNoseBottomHeight.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerNoseBottomHeight.Thickness = _lineThicknessStorable.val * 0.001f;

            // mouth middle
            pos = _vertexMouthMidHeight.Position(this);
            pos.x = midpointX;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthMidHeight.Length = _headSizeWidthStorable.val;
            _markerMouthMidHeight.Origin = pos;
            _markerMouthMidHeight.Enabled = _showFaceMarkersStorable.val;
            _markerMouthMidHeight.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerMouthMidHeight.Thickness = _lineThicknessStorable.val * 0.001f;

            // mouth left
            pos = _vertexMouthLeftSideMiddle.Position(this);
            pos.x = pos.x - _markerLeftRightStorable.val;
            pos.y = _markerMouthMidHeight.Origin.y - (_headSizeHeightStorable.val / 8 / 2);
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthLeft.Length = _headSizeHeightStorable.val / 8;
            _markerMouthLeft.Origin = pos;
            _markerMouthLeft.Enabled = _showFaceMarkersStorable.val;
            _markerMouthLeft.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerMouthLeft.Thickness = _lineThicknessStorable.val * 0.001f;

            // mouth right
            pos = _vertexMouthRightSideMiddle.Position(this);
            pos.x = pos.x - _markerLeftRightStorable.val;
            pos.y = _markerMouthMidHeight.Origin.y - (_headSizeHeightStorable.val / 8 / 2);
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerMouthRight.Length = _headSizeHeightStorable.val / 8;
            _markerMouthRight.Origin = pos;
            _markerMouthRight.Enabled = _showFaceMarkersStorable.val;
            _markerMouthRight.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerMouthRight.Thickness = _lineThicknessStorable.val * 0.001f;

            // chin small and blue
            pos = _vertexHead.Position(this);
            pos.x = pos.x - _markerLeftRightStorable.val + _headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerChinSmall.Length = _headSizeWidthStorable.val;
            _markerChinSmall.Origin = pos;
            _markerChinSmall.Enabled = _showFaceMarkersStorable.val;
            _markerChinSmall.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerChinSmall.Thickness = _lineThicknessStorable.val * 0.001f;

            // head small and blue
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _headSizeHeightStorable.val;
            pos.x = pos.x - _markerLeftRightStorable.val + _headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadSmall.Length = _headSizeWidthStorable.val;
            _markerHeadSmall.Origin = pos;
            _markerHeadSmall.Enabled = _showFaceMarkersStorable.val;
            _markerHeadSmall.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerHeadSmall.Thickness = _lineThicknessStorable.val * 0.001f;

            // head left
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _headSizeHeightStorable.val;
            pos.x = pos.x - _markerLeftRightStorable.val + _headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadLeft.Length = _headSizeHeightStorable.val;
            _markerHeadLeft.Origin = pos;
            _markerHeadLeft.Enabled = _showFaceMarkersStorable.val;
            _markerHeadLeft.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerHeadLeft.Thickness = _lineThicknessStorable.val * 0.001f;

            // head right
            pos = _vertexHead.Position(this);
            pos.y = pos.y - _headSizeHeightStorable.val;
            pos.x = pos.x - _markerLeftRightStorable.val - _headSizeWidthStorable.val / 2;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerHeadRight.Length = _headSizeHeightStorable.val;
            _markerHeadRight.Origin = pos;
            _markerHeadRight.Enabled = _showFaceMarkersStorable.val;
            _markerHeadRight.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerHeadRight.Thickness = _lineThicknessStorable.val * 0.001f;

            // face center
            pos = _vertexNoseTip.Position(this);
            pos.y = pos.y - (pos.y - _markerChin.Origin.y);
            pos.x = pos.x - _markerLeftRightStorable.val;
            pos.z = _markerHead.Origin.z - 0.045f;
            _markerFaceCenter.Length = _headSizeHeightStorable.val;
            _markerFaceCenter.Origin = pos;
            _markerFaceCenter.Enabled = _showFaceMarkersStorable.val;
            _markerFaceCenter.LabelEnabled = _showFeatureMarkerLabelsStorable.val;
            _markerFaceCenter.Thickness = _lineThicknessStorable.val * 0.001f;
        }



        private void UpdateMeasurements() {
            // basic body heights
            _headSizeHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerChin.Origin);
            _headSizeWidthStorable.val = Vector3.Distance(_vertexEarLeft.Position(this), _vertexEarRight.Position(this));

            _fullHeightStorable.val = Vector3.Distance(_markerHead.Origin, _markerHeel.Origin);
            _heightInHeadsStorable.val = _headSizeHeightStorable.val == 0 ? 0 : _fullHeightStorable.val / _headSizeHeightStorable.val;

            _chinHeightStorable.val = Vector3.Distance(_markerChin.Origin, _markerHeel.Origin);
            _shoulderHeightStorable.val = Vector3.Distance(_markerShoulder.Origin, _markerHeel.Origin);
            _nippleHeightStorable.val = Vector3.Distance(_markerNipple.Origin, _markerHeel.Origin);
            _underbustHeightStorable.val = Vector3.Distance(_markerUnderbust.Origin, _markerHeel.Origin);
            _navelHeightStorable.val = Vector3.Distance(_markerNavel.Origin, _markerHeel.Origin);
            _crotchHeightStorable.val = Vector3.Distance(_markerGroin.Origin, _markerHeel.Origin);
            _kneeBottomHeightStorable.val = Vector3.Distance(_markerKnee.Origin, _markerHeel.Origin);

            // measure things around (breast, waist, hip)
            _waistSizeStorable.val = _circumferenceWaist;
            _hipSizeStorable.val = _circumferenceHip;

            var cupInfo = GetCupInfo();
            if(cupInfo == null) {
                _breastBustStorable.val = 0;
                _breastUnderbustStorable.val = 0;
                _breastBandStorable.val = 0;
                _breastCupStorable.val = "";
            }
            else {
                _breastBustStorable.val = cupInfo.Bust;
                _breastUnderbustStorable.val = cupInfo.Underbust;
                _breastBandStorable.val = cupInfo.Band;
                _breastCupStorable.val = cupInfo.Cup;
            }
        }

        private void UpdateMarkerLabels() {

            if(!_showFeatureMarkerLabelsStorable.val) {
                return;
            }

            // update marker labels
            _markerHead.Label = "Head (Head To Heel "
                + $"{(int)(_fullHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_fullHeightStorable.val)} / "
                + $"{_heightInHeadsStorable.val:0.0} heads)";

            var chinToShoulder = _chinHeightStorable.val - _shoulderHeightStorable.val;
            _markerChin.Label = "Chin (Head Height "
                + $"{(int)(_headSizeHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_headSizeHeightStorable.val)}; "
                + $"Width {(int)(_headSizeWidthStorable.val * 100)} cm / {UnitUtils.MetersToFeetString(_headSizeWidthStorable.val)})\n"
                + $"Chin To Shoulder "
                + $"{(int)(chinToShoulder * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(chinToShoulder)} / "
                + $"{chinToShoulder / _headSizeHeightStorable.val:0.0} heads";

            if(_breastBustStorable.val == 0) {
                _markerNipple.Label = "Nipple";
                _markerUnderbust.Label = "Underbust";
            }
            else {
                _markerNipple.Label = $"Nipple "
                + $"(Cup {_breastBandStorable.val}{_breastCupStorable.val}; "
                + $"Around {(int)(_breastBustStorable.val * 100)} cm / "
                + $"{(int)UnitUtils.UnityToInches(_breastBustStorable.val)} in)";

                _markerUnderbust.Label = "Underbust (Around "
                    + $"{(int)(_breastUnderbustStorable.val * 100)} cm / "
                    + $"{(int)UnitUtils.UnityToInches(_breastUnderbustStorable.val)} in)";
            }

            var shoulderToNavel = _shoulderHeightStorable.val - _navelHeightStorable.val;
            _markerNavel.Label = "Navel (Waist "
                + $"{(int)(_waistSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_waistSizeStorable.val))} in)\n"
                + $"Shoulder to Navel "
                + $"{(int)(shoulderToNavel * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(shoulderToNavel)} / "
                + $"{shoulderToNavel / _headSizeHeightStorable.val:0.0} heads";

            var shoulderToCrotch = _shoulderHeightStorable.val - _crotchHeightStorable.val;
            _markerGroin.Label = "Crotch (Hip "
                + $"{(int)(_hipSizeStorable.val * 100)} cm / "
                + $"{Mathf.RoundToInt(UnitUtils.UnityToInches(_hipSizeStorable.val))} in)\n"
                + $"Shoulder to Crotch "
                + $"{(int)(shoulderToCrotch * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(shoulderToCrotch)} / "
                + $"{shoulderToCrotch / _headSizeHeightStorable.val:0.0} heads";

            var crotchToKnee = _crotchHeightStorable.val - _kneeBottomHeightStorable.val;
            _markerKnee.Label = $"Knee Bottom (Crotch to Knee "
                + $"{(int)(crotchToKnee * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(crotchToKnee)} / "
                + $"{crotchToKnee / _headSizeHeightStorable.val:0.0} heads)";

            _markerHeel.Label = $"Heel (Knee to Heel "
                + $"{(int)(_kneeBottomHeightStorable.val * 100)} cm / "
                + $"{UnitUtils.MetersToFeetString(_kneeBottomHeightStorable.val)} / "
                + $"{_kneeBottomHeightStorable.val / _headSizeHeightStorable.val:0.0} heads)";

        }

        private float LineLength(Vector3[] vertices) {
            float total = 0;
            for(var i = 1; i < vertices.Length; i++) {
                var distance = Mathf.Abs(Vector3.Distance(vertices[i-1], vertices[i]));
                total += distance;
            }
            return total;
        }

        List<GameObject> _bustMarkersFromMorph = new List<GameObject>();
        float _circumferenceBust = 0;
        private void UpdateBustMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val) {
                if(_bustMarkersFromMorph.Count != _verticesBust.Length)
                {
                    foreach(var m in _bustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _bustMarkersFromMorph.Clear();
                    foreach(var m in _verticesBust){
                        _bustMarkersFromMorph.Add(CreateMarker(Color.red));
                    }
                }

                for(var i = 0; i < _verticesBust.Length; i++) {
                    _bustMarkersFromMorph[i].transform.position = _verticesBust[i].Position(this);
                }
            }
            else {
                foreach(var m in _bustMarkersFromMorph) {
                    Destroy(m);
                }
                _bustMarkersFromMorph.Clear();
            }

            _circumferenceBust = LineLength(_verticesBust.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _underbustMarkersFromMorph = new List<GameObject>();
        float _circumferenceUnderbust = 0;
        private void UpdateUnderbustMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_underbustMarkersFromMorph.Count != _verticesUnderbust.Length) {
                    foreach(var m in _underbustMarkersFromMorph) {
                        Destroy(m);
                    }
                    _underbustMarkersFromMorph.Clear();
                    foreach(var m in _verticesUnderbust){
                        _underbustMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesUnderbust.Length; i++) {
                    _underbustMarkersFromMorph[i].transform.position = _verticesUnderbust[i].Position(this);
                }
            }
            else {
                foreach(var m in _underbustMarkersFromMorph) {
                    Destroy(m);
                }
                _underbustMarkersFromMorph.Clear();
            }

            _circumferenceUnderbust = LineLength(_verticesUnderbust.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _waistMarkersFromMorph = new List<GameObject>();
        float _circumferenceWaist = 0;
        private void UpdateWaistMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_waistMarkersFromMorph.Count != _verticesWaist.Length) {
                    foreach(var m in _waistMarkersFromMorph) {
                        Destroy(m);
                    }
                    _waistMarkersFromMorph.Clear();
                    foreach(var m in _verticesWaist){
                        _waistMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesWaist.Length; i++) {
                    _waistMarkersFromMorph[i].transform.position = _verticesWaist[i].Position(this);
                }
            }
            else {
                foreach(var m in _waistMarkersFromMorph) {
                    Destroy(m);
                }
                _waistMarkersFromMorph.Clear();
            }

            _circumferenceWaist = LineLength(_verticesWaist.Select(v => v.Position(this)).ToArray()) * 2;
        }

        List<GameObject> _hipMarkersFromMorph = new List<GameObject>();
        float _circumferenceHip = 0;
        private void UpdateHipMarkersFromMorphVertex() {
            if(Skin == null) {
                return;
            }

            if(_showTapeMarkersStorable.val){
                if(_hipMarkersFromMorph.Count != _verticesHip.Length) {
                    foreach(var m in _hipMarkersFromMorph) {
                        Destroy(m);
                    }
                    _hipMarkersFromMorph.Clear();
                    foreach(var m in _verticesHip){
                        _hipMarkersFromMorph.Add(CreateMarker(Color.white));
                    }
                }

                for(var i = 0; i < _verticesHip.Length; i++) {
                    _hipMarkersFromMorph[i].transform.position = _verticesHip[i].Position(this);
                }
            }
            else {
                foreach(var m in _hipMarkersFromMorph) {
                    Destroy(m);
                }
                _hipMarkersFromMorph.Clear();
            }

            _circumferenceHip = LineLength(_verticesHip.Select(v => v.Position(this)).ToArray()) * 2;
        }

        readonly List<HorizontalMarker> _extraHeadMarkers = new List<HorizontalMarker>();
        private void UpdateHeadHeightMarkers() {
            var height = _fullHeightStorable.val;
            var headHeight = _headSizeHeightStorable.val;
            var heightInHeadsRoundedUp = (int)Mathf.Ceil(_heightInHeadsStorable.val);

            if(heightInHeadsRoundedUp != _extraHeadMarkers.Count) {
                if(heightInHeadsRoundedUp > _extraHeadMarkers.Count) {
                    for(var i = _extraHeadMarkers.Count; i < heightInHeadsRoundedUp; i++) {
                        var hm = gameObject.AddComponent<HorizontalMarker>();
                        hm.Name = $"Head{i}";
                        hm.Color = Color.white;
                        hm.LineDirection = Vector3.right;
                        _extraHeadMarkers.Add(hm);
                    }
                }

                for(var i = 0; i < _extraHeadMarkers.Count; i++) {
                    _extraHeadMarkers[i].Enabled = false;
                }
            }

            if(height > 0 && headHeight > 0) {
                for(var i = 0; i < heightInHeadsRoundedUp; i++) {
                    var pos = _vertexHead.Position(this);
                    pos.x += _markerSpreadStorable.val - _markerLeftRightStorable.val;
                    pos.z += _markerFrontBackStorable.val;
                    pos.y -= headHeight * i;

                    _extraHeadMarkers[i].Origin = pos;
                    _extraHeadMarkers[i].Enabled = _showHeadHeightMarkersStorable.val;
                    _extraHeadMarkers[i].Label = $"{i}";
                    _extraHeadMarkers[i].Thickness = _lineThicknessStorable.val * 0.001f;
                }
            }
        }

        private CupSize GetCupInfo() {
            if(_dazCharacter.isMale) {
                return null;
            }
            var cupCalculator = _cupCalculators.FirstOrDefault(cc => cc.Name.Equals(_cupAlgorithmStorable.val));
            if(cupCalculator == null) {
                return null;
            }
            return cupCalculator.Calculate(_circumferenceBust, _circumferenceUnderbust);
        }

        private GameObject CreateMarker(Color color) {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        
            // random position to help avoid physics problems.
            gameObject.transform.position = new Vector3 ((UnityEngine.Random.value*461)+10, (UnityEngine.Random.value*300)+10, 0F);

            // make it smaller
            gameObject.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

            // make it red
            var r = gameObject.GetComponent<Renderer>();
            if(r) {
                r.material.color = color;
            }

            // remove collisions
            foreach(var c in gameObject.GetComponents<Collider>()) {
                Destroy(c);
            }

            return gameObject;
        }

        private string ColliderName(Collider collider)  {
            var parent = collider.attachedRigidbody != null ? collider.attachedRigidbody.name : collider.transform.parent.name;
            var label = parent == collider.name ? collider.name : $"{parent}/{collider.name}";

            return label;
        }
    }
}
