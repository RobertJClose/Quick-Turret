using QuickTurret.Selection;
using QuickTurret.TargetingSystem;
using QuickTurret.TurretComponents;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectedTurretPanel : MonoBehaviour
{
    SelectionManager selectionManager;

    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;
    public Image TurretImage;
    public RectTransform AmmoBeltScrollViewContent;

    public Button ClosestTargetButton;
    public Button ArmouredTargetsButton;
    public Button AvoidArmourButton;
    public Button HuntWeakspotsButton;

    public TagPriorities ArmouredTargetsPriority;
    public TagPriorities AvoidArmourPriority;
    public TagPriorities HuntWeakspotsPriority;

    public Sprite MachineGunSprite;
    public Sprite MinigunSprite;
    public Sprite SniperSprite;

    [TextArea]
    public string MachineGunDesciption;

    [TextArea]
    public string MinigunDescription;

    [TextArea]
    public string SniperDescription;

    public List<AmmoType> MachineGunAmmoChoices = new List<AmmoType>();
    public List<AmmoType> MinigunAmmoChoices = new List<AmmoType>();
    public List<AmmoType> SniperAmmoChoices = new List<AmmoType>();

    private void Awake()
    {
        selectionManager = SelectionManager.Instance;
        TurretImage.preserveAspect = true;
    }

    private void Start()
    {
        selectionManager.OnSelectionChanged += HandleOnSelectionChangedEvent;
    }

    private void OnDestroy()
    {
        selectionManager.OnSelectionChanged -= HandleOnSelectionChangedEvent;
    }

    public void ChangeToArmouredTargeting()
    {
        selectionManager.Selection.GetComponent<TargetScanner>().TargetingPriorities = ArmouredTargetsPriority;
        selectionManager.Selection.GetComponent<TurretController>().ResetTarget();

        SetUpTargetingButtons();
    }

    public void ChangeToAvoidArmour()
    {
        selectionManager.Selection.GetComponent<TargetScanner>().TargetingPriorities = AvoidArmourPriority;
        selectionManager.Selection.GetComponent<TurretController>().ResetTarget();

        SetUpTargetingButtons();
    }

    public void ChangeToClosestTargeting()
    {
        selectionManager.Selection.GetComponent<TargetScanner>().TargetingPriorities = null;
        selectionManager.Selection.GetComponent<TurretController>().ResetTarget();

        SetUpTargetingButtons();
    }

    public void ChangeToHuntWeakspots()
    {
        selectionManager.Selection.GetComponent<TargetScanner>().TargetingPriorities = HuntWeakspotsPriority;
        selectionManager.Selection.GetComponent<TurretController>().ResetTarget();

        SetUpTargetingButtons();
    }

    public void DeselectAndClose()
    {
        selectionManager.SetSelectedGameObject(null);
    }

    private void HandleOnSelectionChangedEvent(object sender, SelectionManager.OnSelectionChangedEventArgs e)
    {
        if (selectionManager.Selection == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);
        string turretName = selectionManager.Selection.name;
        NameText.text = turretName;

        if (turretName.Contains("Machine"))
        {
            DescriptionText.text = MachineGunDesciption;
            TurretImage.sprite = MachineGunSprite;
            SetUpAmmoChoices(3, MachineGunAmmoChoices);
        }
        else if (turretName.Contains("Minigun"))
        {
            DescriptionText.text = MinigunDescription;
            TurretImage.sprite = MinigunSprite;
            SetUpAmmoChoices(5, MinigunAmmoChoices);
        }
        else if (turretName.Contains("Sniper"))
        {
            DescriptionText.text = SniperDescription;
            TurretImage.sprite = SniperSprite;
            SetUpAmmoChoices(1, SniperAmmoChoices);
        }

        SetUpTargetingButtons();
    }

    private void SetUpAmmoChoices(int beltSize, List<AmmoType> ammoChoices)
    {
        for (int i = 0; i < AmmoBeltScrollViewContent.childCount; i++)
        {
            GameObject round = AmmoBeltScrollViewContent.GetChild(i).gameObject;

            if (i + 1 <= beltSize)
            {
                round.SetActive(true);
            }
            else
            {
                round.SetActive(false);
                continue;
            }
            
            Autoloader selectedAutoloader = selectionManager.Selection.GetComponent<Autoloader>();
            TMP_Dropdown roundDropdown = round.GetComponent<TMP_Dropdown>();

            roundDropdown.ClearOptions();
            roundDropdown.AddOptions(ammoChoices.ConvertAll(a => a.AmmoName));
            roundDropdown.SetValueWithoutNotify(ammoChoices.FindIndex(a => a.AmmoName == selectedAutoloader.GetRoundAtIndex(i).AmmoName));

            void ChangeAmmo(int dropdownValue)
            {
                AmmoType ammoChoice = ammoChoices[dropdownValue];

                selectedAutoloader.ChangeAmmo(round.transform.GetSiblingIndex(), ammoChoice);
            }
            
            roundDropdown.onValueChanged.RemoveAllListeners();
            roundDropdown.onValueChanged.AddListener(ChangeAmmo);
        }
    }

    private void SetUpTargetingButtons()
    {
        GameObject selectedTurret = selectionManager.Selection;
        TargetScanner targetScanner = selectedTurret.GetComponent<TargetScanner>();
        TagPriorities priorities = targetScanner.TargetingPriorities;

        if (priorities == null)
        {
            ClosestTargetButton.interactable = false;
            ArmouredTargetsButton.interactable = true;
            AvoidArmourButton.interactable = true;
            HuntWeakspotsButton.interactable = true;
        }
        else if (priorities == ArmouredTargetsPriority)
        {
            ClosestTargetButton.interactable = true;
            ArmouredTargetsButton.interactable = false;
            AvoidArmourButton.interactable = true;
            HuntWeakspotsButton.interactable = true;
        }
        else if (priorities == AvoidArmourPriority)
        {
            ClosestTargetButton.interactable = true;
            ArmouredTargetsButton.interactable = true;
            AvoidArmourButton.interactable = false;
            HuntWeakspotsButton.interactable = true;
        }
        else if (priorities == HuntWeakspotsPriority)
        {
            ClosestTargetButton.interactable = true;
            ArmouredTargetsButton.interactable = true;
            AvoidArmourButton.interactable = true;
            HuntWeakspotsButton.interactable = false;
        }
    }
}
