using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceUIController : MonoBehaviour
{
    [SerializeField] private GameObject _damageIconPrefab;
    [SerializeField] public Slider _EXPSlider;

    [Header("DataSlide")]
    public float current_EXP;
    private Transform _mainCameraTransform;
    private void Awake()
    {

    }

    private void Start()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        var statValue_Update = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Status_UpdateSystem>();
        //statValue_Update.OnUpdateHealth += DisplayDamageIcon;
        GlobalAction.OnUpdateHealth += DisplayDamageIcon;
        GlobalAction.OnUpdateHealth += AudioHit;
        GlobalAction.OnGrantEXP += DisplayEXPSlider;
        GlobalAction.OnLevelUp += OnLevelUP;
        GlobalAction.OnEnemyReceiveHit += OnReceiveDamage;
        // statValue_Update.OnGrantExperience += DisplayExperienceIcon;

    }
    private void OnReceiveDamage(EntityCommandBuffer ecb, Entity entityParent, Entity entityAnimator)
    {

        ecb.AddComponent<SpriteSheetAnimation>(entityAnimator, new SpriteSheetAnimation { indexAnim = 3, maxSprite = 2, _frameCountdown = 0.25f, nextframe = 0.25f, repeatition = SpriteSheetAnimation.RepeatitionType.LOOP, animationFrameIndex = 0 });
        //Wait 2sec?
        ecb.AddComponent<EnemyStateComponent>(entityParent, new EnemyStateComponent { state = EnemyStateComponent.State.ReceiveDamage, statCountDown = 0.5f });
    }
    private void OnDisable()
    {
        //if (World.DefaultGameObjectInjectionWorld == null) return;
        //var statValue_Update = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<Status_UpdateSystem>();
        // statValue_Update.OnUpdateHealth -= DisplayDamageIcon;
        GlobalAction.OnUpdateHealth -= DisplayDamageIcon;
        GlobalAction.OnUpdateHealth -= AudioHit;
    }
    private void AudioHit(float damageAmount, float3 startPosition)
    {
        AudioManager.intances.PlaySFX(AudioManager.SFX.MELEE);
    }
    private void DisplayDamageIcon(float damageAmount, float3 startPosition)
    {
        var directionToCamera = (Vector3)startPosition - _mainCameraTransform.position;
        var rotationToCamera = Quaternion.LookRotation(directionToCamera, Vector3.up);
        var newIcon = Instantiate(_damageIconPrefab, startPosition, Quaternion.identity, transform);
        var newIconText = newIcon.GetComponent<TextMeshProUGUI>();
        newIconText.text = $"<color=red>{damageAmount.ToString()}</color>";
    }

    private void DisplayExperienceIcon(float experienceAmount, float3 startPosition)
    {
        var directionToCamera = (Vector3)startPosition - _mainCameraTransform.position;
        var rotationToCamera = Quaternion.LookRotation(directionToCamera, Vector3.up);
        var newIcon = Instantiate(_damageIconPrefab, startPosition, Quaternion.identity, transform);
        var newIconText = newIcon.GetComponent<TextMeshProUGUI>();
        newIconText.text = $"<color=yellow>{experienceAmount.ToString()} EXP</color>";
    }
    private void DisplayEXPSlider(float experienceAmount)
    {
        current_EXP++;
        _EXPSlider.value = (float)current_EXP / 100;
        if (current_EXP == 100)
        {
            current_EXP = 0;
            _EXPSlider.value = 0;
            GlobalAction.OnLevelUp?.Invoke();

        }
        UnityEngine.Debug.Log("exp");
    }
    private void OnLevelUP()
    {
        //StartCoroutine(wait());
        UnityEngine.Debug.Log("LEvelUp");
    }
    IEnumerator wait()
    {
        yield return new WaitForSeconds(1);
        var similateSystemGroup = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SimulationSystemGroup>();
        similateSystemGroup.Enabled = false;
    }
}