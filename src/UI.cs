using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace LFE {
    public class UI {

        private readonly ICupCalculator[] _cupCalculators = new ICupCalculator[] {
            new EvasIntimatesCupCalculator(),
            new KnixComCupCalculator(),
            new SizeChartCupCalculator(),
            new ChateLaineCupCalculator(),
            new KSK9404CupCalculator()
        };

        public List<Proportions> proportionTemplates = new List<Proportions>();


        private static Color HEADER_COLOR = new Color(0, 0, 0, 0.25f);
        private static Color SPACER_COLOR = new Color(0, 0, 0, 0.5f);

        private readonly HeightMeasurePlugin _plugin;

        // measurement storables
        public JSONStorableFloat fullHeightStorable;
        public JSONStorableFloat headSizeHeightStorable;
        public JSONStorableFloat headSizeWidthStorable;
        public JSONStorableFloat chinHeightStorable;
        public JSONStorableFloat shoulderHeightStorable;
        public JSONStorableFloat nippleHeightStorable;
        public JSONStorableFloat underbustHeightStorable;
        public JSONStorableFloat navelHeightStorable;
        public JSONStorableFloat crotchHeightStorable;
        public JSONStorableFloat kneeBottomHeightStorable;
        public JSONStorableFloat heightInHeadsStorable;
        public JSONStorableFloat breastBustStorable;
        public JSONStorableFloat breastUnderbustStorable;
        public JSONStorableFloat breastBandStorable;
        public JSONStorableString breastCupStorable;
        public JSONStorableFloat waistSizeStorable;
        public JSONStorableFloat hipSizeStorable;
        public JSONStorableFloat penisLength;
        public JSONStorableFloat penisWidth;
        public JSONStorableFloat penisGirth;

        public JSONStorableFloat ageFromHeadStorable;
        public JSONStorableFloat ageFromHeightStorable;
        public JSONStorableString proportionClosestMatch;

        // TOOD: shoulder width

        // manual heights
        public JSONStorableBool showManualMarkersStorable;
        public JSONStorableBool manualMarkersCopy;
        public JSONStorableColor manualMarkerColor;
        public JSONStorableFloat lineThicknessManualStorable;

        public JSONStorableFloat manualHeightStorable;
        public JSONStorableFloat manualChinHeightStorable;
        public JSONStorableFloat manualShoulderHeightStorable;
        public JSONStorableFloat manualNippleHeightStorable;
        public JSONStorableFloat manualUnderbustHeightStorable;
        public JSONStorableFloat manualNavelHeightStorable;
        public JSONStorableFloat manualCrotchHeightStorable;
        public JSONStorableFloat manualKneeBottomHeightStorable;


        // other storables
        public JSONStorableBool showFeatureMarkersStorable;
        public JSONStorableColor featureMarkerColor;
        public JSONStorableFloat lineThicknessFigureStorable;
        public JSONStorableBool showFeatureMarkerLabelsStorable;

        public JSONStorableBool showHeadHeightMarkersStorable;
        public JSONStorableColor headMarkerColor;
        public JSONStorableFloat lineThicknessHeadStorable;

        public JSONStorableBool showFaceMarkersStorable;
        public JSONStorableColor faceMarkerColor;
        public JSONStorableFloat lineThicknessFaceStorable;

        public JSONStorableBool showCircumferenceMarkersStorable;
        public JSONStorableColor circumferenceMarkerColor;
        public JSONStorableFloat lineThicknessCircumferenceStorable;

        public JSONStorableBool showProportionMarkersStorable;
        public JSONStorableColor proportionMarkerColor;
        public JSONStorableFloat lineThicknessProportionStorable;
        public JSONStorableStringChooser proportionSelectionStorable;

        public JSONStorableBool showTargetHeadRatioStorable;
        public JSONStorableFloat targetHeadRatioStorable;
        public JSONStorableStringChooser targetHeadRatioMorphStorable;

        public JSONStorableStringChooser cupAlgorithmStorable;
        public JSONStorableStringChooser unitsStorable;
        public JSONStorableFloat markerSpreadStorable;
        public JSONStorableFloat markerLeftRightStorable;
        public JSONStorableFloat markerFrontBackStorable;
        public JSONStorableFloat markerUpDownStorable;
        public JSONStorableBool hideDocsStorable;

        public ICupCalculator CupCalculator => _cupCalculators.FirstOrDefault(c => c.Name.Equals(cupAlgorithmStorable.val));

        public UI(HeightMeasurePlugin plugin) {
            _plugin = plugin;
            proportionTemplates = Proportions.CommonProportions;
            InitStorables();
            Draw();
        }

        private bool _choosingFeatureColor = false;
        private bool _choosingHeadColor = false;
        private bool _choosingFaceColor = false;
        private bool _choosingCircumferenceColor = false;
        private bool _choosingManualColor = false;
        private bool _choosingProportionColor = false;
        private void InitStorables() {

            //////////////////////////////////////
            // UI related
            // Cup algorithm choice

            showFeatureMarkersStorable = new JSONStorableBool("Auto Feature Guides", false, (bool value) => {
                showFeatureMarkersStorable.valNoCallback = value;
                Draw();
            });
            showFeatureMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkersStorable);

            lineThicknessFigureStorable = new JSONStorableFloat("Feature Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessFigureStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessFigureStorable);

            featureMarkerColor = new JSONStorableColor("Feature Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                featureMarkerColor.valNoCallback = hsv;
                if(_featureMarkerColorButton != null) {
                    _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            featureMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(featureMarkerColor);

            showFeatureMarkerLabelsStorable = new JSONStorableBool("Feature Guide Labels", true, (bool value) => {
                Draw();
            });
            showFeatureMarkerLabelsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFeatureMarkerLabelsStorable);

            //////////////////

            showHeadHeightMarkersStorable = new JSONStorableBool("Auto Head Heights Guides", true, (bool value) => {
                showHeadHeightMarkersStorable.valNoCallback = value;
                Draw();
            });
            showHeadHeightMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showHeadHeightMarkersStorable);

            lineThicknessHeadStorable = new JSONStorableFloat("Head Heights Line Width", 2, 1, 10, constrain: true);
            lineThicknessHeadStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessHeadStorable);

            headMarkerColor = new JSONStorableColor("Head Guide Color", _plugin.ColorToHSV(Color.white), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                headMarkerColor.valNoCallback = hsv;
                if(_headMarkerColorButton != null) {
                    _headMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            headMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(headMarkerColor);

            //////////////////

            showFaceMarkersStorable = new JSONStorableBool("Auto Face Guides", false, (bool value) => {
                Draw();
            });
            showFaceMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showFaceMarkersStorable);

            lineThicknessFaceStorable = new JSONStorableFloat("Face Line Width", 2, 1, 10, constrain: true);
            lineThicknessFaceStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessFaceStorable);

            faceMarkerColor = new JSONStorableColor("Face Guide Color", _plugin.ColorToHSV(Color.blue), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                faceMarkerColor.valNoCallback = hsv;
                if(_faceMarkerColorButton != null) {
                    _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            faceMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(faceMarkerColor);

            //////////////////

            showCircumferenceMarkersStorable = new JSONStorableBool("Auto Circumference Guides", false, (bool value) => {
                showCircumferenceMarkersStorable.valNoCallback = value;
                Draw();
            });
            showCircumferenceMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showCircumferenceMarkersStorable);

            lineThicknessCircumferenceStorable = new JSONStorableFloat("Circumference Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessCircumferenceStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessCircumferenceStorable);

            circumferenceMarkerColor = new JSONStorableColor("Circumference Guide Color", _plugin.ColorToHSV(Color.green), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                circumferenceMarkerColor.valNoCallback = hsv;
                if(_circumferenceMarkerColorButton != null) {
                    _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            circumferenceMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(circumferenceMarkerColor);

            //////////////////
            showProportionMarkersStorable = new JSONStorableBool("Proportion Guides", false, (bool value) => {
                Draw();
            });
            showProportionMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showProportionMarkersStorable);

            lineThicknessProportionStorable = new JSONStorableFloat("Proportion Line Width", 2, 1, 10, constrain: true);
            lineThicknessProportionStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessProportionStorable);

            proportionMarkerColor = new JSONStorableColor("Proportion Guide Color", _plugin.ColorToHSV(Color.yellow), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                proportionMarkerColor.valNoCallback = hsv;
                if(_proportionMarkerColorButton != null) {
                    _proportionMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            proportionMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(proportionMarkerColor);

            var proportionNames = proportionTemplates.Select(p => p.ProportionName).ToList();
            proportionNames.Insert(0, "Auto Detect");
            proportionSelectionStorable = new JSONStorableStringChooser(
                "Proportion Template",
                proportionNames,
                proportionNames[0],
                "Proportion Template"
            );
            proportionSelectionStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(proportionSelectionStorable);

            showTargetHeadRatioStorable = new JSONStorableBool("Enable Auto Head Ratio Targeting", false);
            showTargetHeadRatioStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showTargetHeadRatioStorable);

            targetHeadRatioStorable = new JSONStorableFloat("Head Ratio Target", 7.6f, 0, 10);
            targetHeadRatioStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(targetHeadRatioStorable);

            targetHeadRatioMorphStorable = new JSONStorableStringChooser(
                "Head Ratio Morph",
                new List<string> { "Head big", "Head Scale" },
                "Head big",
                "Prefer Morph"
            );
            targetHeadRatioMorphStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(targetHeadRatioMorphStorable);

            // Choose
            cupAlgorithmStorable = new JSONStorableStringChooser(
                "Cup Size Method",
                _cupCalculators.Select(cc => cc.Name).ToList(),
                _cupCalculators[0].Name,
                "Cup Size Method"
            );
            cupAlgorithmStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(cupAlgorithmStorable);

            unitsStorable = new JSONStorableStringChooser(
                "Units",
                new List<string> { UnitUtils.US, UnitUtils.Metric, UnitUtils.Meters, UnitUtils.Centimeters, UnitUtils.Inches, UnitUtils.Feet, UnitUtils.Heads },
                new List<string> { "US (in / ft)", "Metric (cm / m)", "Meters (m)", "Centimeters (cm)", "Inches (in)", "Feet (ft)", "Heads (head.unit.)"},
                UnitUtils.Centimeters, 
                "Units"
            );
            unitsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterStringChooser(unitsStorable);

            // Float: How far apart markers are spread apart
            markerSpreadStorable = new JSONStorableFloat("Spread Guides", 0.02f, -1, 1);
            markerSpreadStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerSpreadStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerFrontBackStorable = new JSONStorableFloat("Move Guides Forward/Backward", 0.15f, -5, 5);
            markerFrontBackStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerFrontBackStorable);

            // Float: Move markers left or right 
            markerLeftRightStorable = new JSONStorableFloat("Move Guides Left/Right", 0, -5, 5);
            markerLeftRightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerLeftRightStorable);

            // Float: Move markers forward or backward relative to center depth of the head
            markerUpDownStorable = new JSONStorableFloat("Move Guides Up/Down", 0, -5, 5);
            markerUpDownStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(markerUpDownStorable);

            hideDocsStorable = new JSONStorableBool("Hide Documentation", false);
            hideDocsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(hideDocsStorable);


            //////////////////////////////////////

            // calculated positions and distances for other _plugins to use if wanted
            fullHeightStorable = new JSONStorableFloat("figureHeight", 0, 0, 100);
            fullHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(fullHeightStorable);

            heightInHeadsStorable = new JSONStorableFloat("figureHeightHeads", 0, 0, 100);
            heightInHeadsStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(heightInHeadsStorable);

            headSizeHeightStorable = new JSONStorableFloat("headSizeHeight", 0, 0, 100);
            headSizeHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(headSizeHeightStorable);

            chinHeightStorable = new JSONStorableFloat("chinHeight", 0, 0, 100);
            chinHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(chinHeightStorable);

            shoulderHeightStorable = new JSONStorableFloat("shoulderHeight", 0, 0, 100);
            shoulderHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(shoulderHeightStorable);

            nippleHeightStorable = new JSONStorableFloat("nippleHeight", 0, 0, 100);
            nippleHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(nippleHeightStorable);

            underbustHeightStorable = new JSONStorableFloat("underbustHeight", 0, 0, 100);
            underbustHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(underbustHeightStorable);

            navelHeightStorable = new JSONStorableFloat("navelHeight", 0, 0, 100);
            navelHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(navelHeightStorable);

            crotchHeightStorable = new JSONStorableFloat("crotchHeight", 0, 0, 100);
            crotchHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(crotchHeightStorable);

            kneeBottomHeightStorable = new JSONStorableFloat("kneeHeight", 0, 0, 100);
            kneeBottomHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(kneeBottomHeightStorable);

            headSizeWidthStorable = new JSONStorableFloat("headSizeWidth", 0, 0, 100);
            headSizeWidthStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(headSizeWidthStorable);

            breastBustStorable = new JSONStorableFloat("breastBustSize", 0, 0, 100);
            breastBustStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastBustStorable);

            breastUnderbustStorable = new JSONStorableFloat("breastUnderbustSize", 0, 0, 100);
            breastUnderbustStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastUnderbustStorable);

            breastBandStorable = new JSONStorableFloat("breastBandSize", 0, 0, 100);
            breastBandStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(breastBandStorable);

            breastCupStorable = new JSONStorableString("breastCupSize", "");
            breastCupStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterString(breastCupStorable);

            waistSizeStorable = new JSONStorableFloat("waistSize", 0, 0, 100);
            waistSizeStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(waistSizeStorable);

            hipSizeStorable = new JSONStorableFloat("hipSize", 0, 0, 100);
            hipSizeStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(hipSizeStorable);

            penisLength = new JSONStorableFloat("penisLength", 0, 0, 100);
            penisLength.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisLength);

            penisWidth = new JSONStorableFloat("penisWidth", 0, 0, 100);
            penisWidth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisWidth);

            penisGirth = new JSONStorableFloat("penisGirth", 0, 0, 100);
            penisGirth.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(penisGirth);

            ageFromHeadStorable = new JSONStorableFloat("ageFromHead", 0, 0, 100);
            ageFromHeadStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageFromHeadStorable);

            ageFromHeightStorable = new JSONStorableFloat("ageFromHeight", 0, 0, 100);
            ageFromHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(ageFromHeightStorable);

            proportionClosestMatch = new JSONStorableString("proportionMatch", "");
            proportionClosestMatch.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterString(proportionClosestMatch);

            // manual heights
            showManualMarkersStorable = new JSONStorableBool("Manual Markers", false, (bool value) => {
                showManualMarkersStorable.valNoCallback = value;
                Draw();
            });
            showManualMarkersStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(showManualMarkersStorable);

            manualMarkersCopy = new JSONStorableBool("Copy Markers From Person", false);
            manualMarkersCopy.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterBool(manualMarkersCopy);

            lineThicknessManualStorable = new JSONStorableFloat("Manual Guide Line Width", 2, 1, 10, constrain: true);
            lineThicknessManualStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(lineThicknessManualStorable);

            manualMarkerColor = new JSONStorableColor("Manual Guide Color", _plugin.ColorToHSV(Color.yellow), (float h, float s, float v) => {
                var hsv = new HSVColor { H = h, S = s, V = v };
                manualMarkerColor.valNoCallback = hsv;
                if(_manualMarkerColorButton != null) {
                    _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(hsv);
                }
            });
            manualMarkerColor.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterColor(manualMarkerColor);

            manualHeightStorable = new JSONStorableFloat("Height", 0, 0, 300);
            manualHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualHeightStorable);

            manualChinHeightStorable = new JSONStorableFloat("Chin Height", 0, 0, 300);
            manualChinHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualChinHeightStorable);

            manualShoulderHeightStorable = new JSONStorableFloat("Shoulder Height", 0, 0, 300);
            manualShoulderHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualShoulderHeightStorable);

            manualNippleHeightStorable = new JSONStorableFloat("Bust Height", 0, 0, 300);
            manualNippleHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualNippleHeightStorable);

            manualUnderbustHeightStorable = new JSONStorableFloat("Underbust Height", 0, 0, 300);
            manualUnderbustHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualUnderbustHeightStorable);

            manualNavelHeightStorable = new JSONStorableFloat("Navel Height", 0, 0, 300);
            manualNavelHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualNavelHeightStorable);

            manualCrotchHeightStorable = new JSONStorableFloat("Crotch Height", 0, 0, 300);
            manualCrotchHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualCrotchHeightStorable);

            manualKneeBottomHeightStorable = new JSONStorableFloat("Knee Height", 0, 0, 300);
            manualKneeBottomHeightStorable.storeType = JSONStorableParam.StoreType.Full;
            _plugin.RegisterFloat(manualKneeBottomHeightStorable);
        }

        private UIDynamicToggle _featureMarkerToggle;
        private UIDynamicButton _featureMarkerColorButton;
        private UIDynamicColorPicker _featureMarkerColorPicker;
        private UIDynamicSlider _featureMarkerLineThickness;
        private UIDynamicToggle _featureMarkerToggleLabels;

        private UIDynamicToggle _headMarkerToggle;
        private UIDynamicButton _headMarkerColorButton;
        private UIDynamicColorPicker _headMarkerColorPicker;
        private UIDynamicSlider _headMarkerLineThickness;

        private UIDynamicToggle _faceMarkerToggle;
        private UIDynamicButton _faceMarkerColorButton;
        private UIDynamicColorPicker _faceMarkerColorPicker;
        private UIDynamicSlider _faceMarkerLineThickness;

        private UIDynamicToggle _circumferenceMarkerToggle;
        private UIDynamicButton _circumferenceMarkerColorButton;
        private UIDynamicColorPicker _circumferenceMarkerColorPicker;
        private UIDynamicSlider _circumferenceMarkerLineThickness;

        private UIDynamicToggle _proportionMarkerToggle;
        private UIDynamicButton _proportionMarkerColorButton;
        private UIDynamicColorPicker _proportionMarkerColorPicker;
        private UIDynamicSlider _proportionMarkerLineThickness;
        private UIDynamicPopup _proportionSelection;
        private List<UIDynamicSlider> _proportionEditSliders = new List<UIDynamicSlider>();

        private UIDynamicToggle _targetHeadRatioToggle;
        private UIDynamicSlider _targetHeadRatioSlider;
        private UIDynamicPopup _targetHeadRatioMorph;

        private UIDynamicPopup _cupAlgorithm;
        private UIDynamicPopup _units;
        private UIDynamicSlider _markerSpread;
        private UIDynamicSlider _markerFrontBack;
        private UIDynamicSlider _markerLeftRight;
        private UIDynamicSlider _markerUpDown;
        private UIDynamicToggle _hideDocs;

        private UIDynamicToggle _manualMarkerToggle;
        private UIDynamicToggle _copyManualMarkers;
        private UIDynamicButton _manualMarkerColorButton;
        private UIDynamicColorPicker _manualMarkerColorPicker;
        private UIDynamicSlider _manualMarkerLineThickness;
        private List<UIDynamicSlider> _manualSliders = new List<UIDynamicSlider>();

        public void Draw() {
            Clear();

            int defaultButtonHeight = 50;
            int defaultSliderHeight = 120;
            int defaultSectionSpacerHeight = 0;

            if(_plugin.containingAtom.type == "Person") {
                // Head Height Guides
                CreateStandardDivider(rightSide: false);
                _headMarkerToggle = _plugin.CreateToggle(showHeadHeightMarkersStorable, rightSide: false);
                _headMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showHeadHeightMarkersStorable.val) {
                    _headMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: false);
                    _headMarkerColorButton.buttonColor = _plugin.HSVToColor(headMarkerColor.val);
                    _headMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingHeadColor = !_choosingHeadColor;
                        Draw();
                    });
                    if(_choosingHeadColor) {
                        _headMarkerColorPicker = _plugin.CreateColorPicker(headMarkerColor, rightSide: false);
                    }
                    _headMarkerLineThickness = _plugin.CreateSlider(lineThicknessHeadStorable, rightSide: false);
                    CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                }
                else {
                    // if the right side is expanded, add some height padding
                    if(showFeatureMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Feature Guides
                CreateStandardDivider(rightSide: true);
                _featureMarkerToggle = _plugin.CreateToggle(showFeatureMarkersStorable, rightSide: true);
                _featureMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showFeatureMarkersStorable.val) {
                    _featureMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                    _featureMarkerColorButton.buttonColor = _plugin.HSVToColor(featureMarkerColor.val);
                    _featureMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingFeatureColor = !_choosingFeatureColor;
                        Draw();
                    });
                    if(_choosingFeatureColor) {
                        _featureMarkerColorPicker = _plugin.CreateColorPicker(featureMarkerColor, rightSide: true);
                    }
                    _featureMarkerLineThickness = _plugin.CreateSlider(lineThicknessFigureStorable, rightSide: true);
                    _featureMarkerToggleLabels = _plugin.CreateToggle(showFeatureMarkerLabelsStorable, rightSide: true);
                }
                else {
                    // if the left side is expanded, add some height padding
                    if(showHeadHeightMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

                // Face Guides
                CreateStandardDivider(rightSide: false);
                _faceMarkerToggle = _plugin.CreateToggle(showFaceMarkersStorable, rightSide: false);
                _faceMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showFaceMarkersStorable.val) {
                    _faceMarkerColorButton = _plugin.CreateButton("Set Line Color");
                    _faceMarkerColorButton.buttonColor = _plugin.HSVToColor(faceMarkerColor.val);
                    _faceMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingFaceColor = !_choosingFaceColor;
                        Draw();
                    });
                    if(_choosingFaceColor) {
                        _faceMarkerColorPicker = _plugin.CreateColorPicker(faceMarkerColor, rightSide: false);
                    }
                    _faceMarkerLineThickness = _plugin.CreateSlider(lineThicknessFaceStorable, rightSide: false);
                    CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                }
                else {
                    // if right side is expanded
                    if(showCircumferenceMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: false);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);

                // Circumference Guides
                CreateStandardDivider(rightSide: true);
                _circumferenceMarkerToggle = _plugin.CreateToggle(showCircumferenceMarkersStorable, rightSide: true);
                _circumferenceMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showCircumferenceMarkersStorable.val) {
                    _circumferenceMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                    _circumferenceMarkerColorButton.buttonColor = _plugin.HSVToColor(circumferenceMarkerColor.val);
                    _circumferenceMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingCircumferenceColor = !_choosingCircumferenceColor;
                        Draw();
                    });
                    if(_choosingCircumferenceColor) {
                        _circumferenceMarkerColorPicker = _plugin.CreateColorPicker(circumferenceMarkerColor, rightSide: true);
                    }
                    _circumferenceMarkerLineThickness = _plugin.CreateSlider(lineThicknessCircumferenceStorable, rightSide: true);
                    CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                }
                else {
                    // if the left side is expanded, add some height padding
                    if(showFaceMarkersStorable.val) {
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                        CreateStandardSpacer(defaultSliderHeight, rightSide: true);
                        CreateStandardSpacer(defaultButtonHeight, rightSide: true);
                    }
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: true);

                // Proportion Guides
                CreateStandardDivider(rightSide: false);
                _proportionMarkerToggle = _plugin.CreateToggle(showProportionMarkersStorable, rightSide: false);
                _proportionMarkerToggle.backgroundColor = HEADER_COLOR;
                if(showProportionMarkersStorable.val) {
                    _proportionMarkerColorButton = _plugin.CreateButton("Set Line Color");
                    _proportionMarkerColorButton.buttonColor = _plugin.HSVToColor(proportionMarkerColor.val);
                    _proportionMarkerColorButton.button.onClick.AddListener(() => {
                        _choosingProportionColor = !_choosingProportionColor;
                        Draw();
                    });
                    if(_choosingProportionColor) {
                        _proportionMarkerColorPicker = _plugin.CreateColorPicker(proportionMarkerColor, rightSide: false);
                    }
                    _proportionMarkerLineThickness = _plugin.CreateSlider(lineThicknessProportionStorable, rightSide: false);

                    _proportionSelection = _plugin.CreateScrollablePopup(proportionSelectionStorable, rightSide: false);
                    // _proportionSelection.popup.onValueChangeHandlers += (e) => {
                    //     // SuperController.LogMessage($"value changed {e}");
                    //     Draw();
                    // };

                    // var selectedProportion = Proportions.CommonProportions.FirstOrDefault(p => p.ProportionName.Equals(proportionSelectionStorable.val));
                    // if(selectedProportion != null) {
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Height in Heads", selectedProportion.FigureHeightInHeads, (value) => {
                    //             selectedProportion.FigureHeightInHeads = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Chin to Shoulder", selectedProportion.FigureChinToShoulder, (value) => {
                    //             selectedProportion.FigureChinToShoulder = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Nipples", selectedProportion.FigureShoulderToNipples, (value) => {
                    //             selectedProportion.FigureShoulderToNipples = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Navel", selectedProportion.FigureShoulderToNavel, (value) => {
                    //             selectedProportion.FigureShoulderToNavel = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Shoulder to Crotch", selectedProportion.FigureShoulderToCrotch, (value) => {
                    //             selectedProportion.FigureShoulderToCrotch = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Full Leg Length", selectedProportion.FigureLengthOfLowerLimb, (value) => {
                    //             selectedProportion.FigureLengthOfLowerLimb = value;
                    //         }, 0, 10))
                    //     );
                    //     _proportionEditSliders.Add(
                    //         _plugin.CreateSlider(new JSONStorableFloat("Crotch to Knee", selectedProportion.FigureCrotchToBottomOfKnees, (value) => {
                    //             selectedProportion.FigureCrotchToBottomOfKnees = value;
                    //         }, 0, 10))
                    //     );
                    // }

                    CreateStandardSpacer(defaultButtonHeight, rightSide: false);
                }
                CreateStandardSpacer(defaultSectionSpacerHeight, rightSide: false);
            }

            CreateStandardDivider(rightSide: false);
            _targetHeadRatioToggle = _plugin.CreateToggle(showTargetHeadRatioStorable, rightSide: false);
            _targetHeadRatioSlider = _plugin.CreateSlider(targetHeadRatioStorable, rightSide: false);
            _targetHeadRatioMorph = _plugin.CreateScrollablePopup(targetHeadRatioMorphStorable, rightSide: false);

            CreateStandardDivider(rightSide: false);
            _cupAlgorithm = _plugin.CreateScrollablePopup(cupAlgorithmStorable, rightSide: false);
            _units = _plugin.CreateScrollablePopup(unitsStorable, rightSide: false);
            _markerSpread = _plugin.CreateSlider(markerSpreadStorable, rightSide: false);
            _markerFrontBack = _plugin.CreateSlider(markerFrontBackStorable, rightSide: false);
            _markerLeftRight = _plugin.CreateSlider(markerLeftRightStorable, rightSide: false);
            _markerUpDown = _plugin.CreateSlider(markerUpDownStorable, rightSide: false);
            _hideDocs = _plugin.CreateToggle(hideDocsStorable, rightSide: false);

            CreateStandardDivider(rightSide: true);
            _manualMarkerToggle = _plugin.CreateToggle(showManualMarkersStorable, rightSide: true);
            if(showManualMarkersStorable.val) {
                if(_plugin.containingAtom.type == "Person") {
                    _copyManualMarkers = _plugin.CreateToggle(manualMarkersCopy, rightSide: true);
                }
                _manualMarkerColorButton = _plugin.CreateButton("Set Line Color", rightSide: true);
                _manualMarkerColorButton.buttonColor = _plugin.HSVToColor(manualMarkerColor.val);
                _manualMarkerColorButton.button.onClick.AddListener(() => {
                    _choosingManualColor = !_choosingManualColor;
                    Draw();
                });
                if(_choosingManualColor) {
                    _manualMarkerColorPicker = _plugin.CreateColorPicker(manualMarkerColor, rightSide: true);
                }
                _manualMarkerLineThickness = _plugin.CreateSlider(lineThicknessManualStorable, rightSide: true);
                _manualSliders.Add(_plugin.CreateSlider(manualHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualChinHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualShoulderHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualNippleHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualUnderbustHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualNavelHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualCrotchHeightStorable, rightSide: true));
                _manualSliders.Add(_plugin.CreateSlider(manualKneeBottomHeightStorable, rightSide: true));
            }
        }

        public void Clear() {
            foreach(var spacer in _spacerList) {
                _plugin.RemoveSpacer(spacer);
            }
            foreach(var spacer in _spacerLinesList) {
                _plugin.RemoveTextField(spacer);
            }
            if(_manualMarkerToggle) {
                _plugin.RemoveToggle(_manualMarkerToggle);
            }
            if(_copyManualMarkers) {
                _plugin.RemoveToggle(_copyManualMarkers);
            }
            if(_manualMarkerColorButton) {
                _plugin.RemoveButton(_manualMarkerColorButton);
            }
            if(_manualMarkerColorPicker) {
                _plugin.RemoveColorPicker(_manualMarkerColorPicker);
            }
            if(_manualMarkerLineThickness) {
                _plugin.RemoveSlider(_manualMarkerLineThickness);
            }
            foreach(var s in _manualSliders) {
                _plugin.RemoveSlider(s);
            }
            _manualSliders = new List<UIDynamicSlider>();
            if(_featureMarkerToggle) {
                _plugin.RemoveToggle(_featureMarkerToggle);
            }
            if(_featureMarkerColorButton) {
                _plugin.RemoveButton(_featureMarkerColorButton);
            }
            if(_featureMarkerColorPicker) {
                _plugin.RemoveColorPicker(_featureMarkerColorPicker);
            }
            if(_featureMarkerLineThickness) {
                _plugin.RemoveSlider(_featureMarkerLineThickness);
            }
            if(_featureMarkerToggleLabels) {
                _plugin.RemoveToggle(_featureMarkerToggleLabels);
            }

            if(_headMarkerToggle) {
                _plugin.RemoveToggle(_headMarkerToggle);
            }
            if(_headMarkerColorButton) {
                _plugin.RemoveButton(_headMarkerColorButton);
            }
            if(_headMarkerColorPicker) {
                _plugin.RemoveColorPicker(_headMarkerColorPicker);
            }
            if(_headMarkerLineThickness) {
                _plugin.RemoveSlider(_headMarkerLineThickness);
            }

            if(_faceMarkerToggle) {
                _plugin.RemoveToggle(_faceMarkerToggle);
            }
            if(_faceMarkerColorButton) {
                _plugin.RemoveButton(_faceMarkerColorButton);
            }
            if(_faceMarkerColorPicker) {
                _plugin.RemoveColorPicker(_faceMarkerColorPicker);
            }
            if(_faceMarkerLineThickness) {
                _plugin.RemoveSlider(_faceMarkerLineThickness);
            }

            if(_circumferenceMarkerToggle) {
                _plugin.RemoveToggle(_circumferenceMarkerToggle);
            }
            if(_circumferenceMarkerColorButton) {
                _plugin.RemoveButton(_circumferenceMarkerColorButton);
            }
            if(_circumferenceMarkerColorPicker) {
                _plugin.RemoveColorPicker(_circumferenceMarkerColorPicker);
            }
            if(_circumferenceMarkerLineThickness) {
                _plugin.RemoveSlider(_circumferenceMarkerLineThickness);
            }

            if(_proportionMarkerToggle) {
                _plugin.RemoveToggle(_proportionMarkerToggle);
            }
            if(_proportionMarkerColorButton) {
                _plugin.RemoveButton(_proportionMarkerColorButton);
            }
            if(_proportionMarkerColorPicker) {
                _plugin.RemoveColorPicker(_proportionMarkerColorPicker);
            }
            if(_proportionMarkerLineThickness) {
                _plugin.RemoveSlider(_proportionMarkerLineThickness);
            }
            if(_proportionSelection) {
                _plugin.RemovePopup(_proportionSelection);
            }
            foreach(var s in _proportionEditSliders) {
                _plugin.RemoveSlider(s);
            }
            _proportionEditSliders = new List<UIDynamicSlider>();

            if(_targetHeadRatioToggle) {
                _plugin.RemoveToggle(_targetHeadRatioToggle);
            }
            if(_targetHeadRatioSlider) {
                _plugin.RemoveSlider(_targetHeadRatioSlider);
            }
            if(_targetHeadRatioMorph) {
                _plugin.RemovePopup(_targetHeadRatioMorph);
            }

            if(_cupAlgorithm) {
                _plugin.RemovePopup(_cupAlgorithm);
            }
            if(_units) {
                _plugin.RemovePopup(_units);
            }
            if(_markerSpread) {
                _plugin.RemoveSlider(_markerSpread);
            }
            if(_markerFrontBack) {
                _plugin.RemoveSlider(_markerFrontBack);
            }
            if(_markerLeftRight) {
                _plugin.RemoveSlider(_markerLeftRight);
            }
            if(_markerUpDown) {
                _plugin.RemoveSlider(_markerUpDown);
            }
            if(_hideDocs) {
                _plugin.RemoveToggle(_hideDocs);
            }
        }

        private readonly List<UIDynamic> _spacerList = new List<UIDynamic>();
        private void CreateStandardSpacer(float height, bool rightSide = false) {
            var spacer = _plugin.CreateSpacer(rightSide: rightSide);
            spacer.height = height;
            _spacerList.Add(spacer);
        }

        private readonly List<UIDynamicTextField> _spacerLinesList = new List<UIDynamicTextField>();
        private void CreateStandardDivider(bool rightSide = false) {
            var textField = _plugin.CreateTextField(new JSONStorableString("", ""), rightSide: rightSide);
            textField.backgroundColor = SPACER_COLOR;
            var layoutElement = textField.GetComponent<LayoutElement>();
            if(layoutElement != null) {
                layoutElement.minHeight = 0f;
                layoutElement.preferredHeight = 5;
            }
            // remove the scrollbar
            var scrollbar = textField.GetComponentInChildren<Scrollbar>();
            if(scrollbar) {
                UnityEngine.Object.Destroy(scrollbar);
            }
            _spacerLinesList.Add(textField);
        }
    }
}