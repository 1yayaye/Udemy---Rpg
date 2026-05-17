using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public class Clone_Skill : Skill
{


    [Header("Clone info")]
    [SerializeField] private float attackMultiplier;
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration;
    [Space]

    [Header("Clone attack")]
    [SerializeField] private UI_SkillTreeSlot cloneAttackUnlockButton;
    [SerializeField] private float cloneAttackMultiplier;
    [SerializeField] private bool canAttack;

    [Header("Aggresive clone")]
    [SerializeField] private UI_SkillTreeSlot aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMultiplier;
    public bool canApplyOnHitEffect { get; private set; }

    [Header("Multiple clone")]
    [SerializeField] private UI_SkillTreeSlot multipleUnlockButton;
    [SerializeField] private float multiCloneAttackMultiplier;
    [SerializeField] private bool canDuplicateClone;
    [SerializeField] private float chanceToDuplicate;
    [Header("Crystal instead of clone")]
    [SerializeField] private UI_SkillTreeSlot crystalInseadUnlockButton;
    public bool crystalInseadOfClone;
    private float defaultAttackMultiplier;


    protected override void Start()
    {
        defaultAttackMultiplier = attackMultiplier;

        base.Start();


        cloneAttackUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneAttack);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggresiveClone);
        multipleUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiClone);
        crystalInseadUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    #region Unlock region
    protected override void CheckUnlock()
    {
        attackMultiplier = defaultAttackMultiplier;

        UnlockCloneAttack();
        UnlockAggresiveClone();
        UnlockMultiClone();
        UnlockCrystalInstead();
    }

    private void UnlockCloneAttack()
    {
        canAttack = cloneAttackUnlockButton.unlocked;

        if (canAttack)
            attackMultiplier = cloneAttackMultiplier;
    }

    private void UnlockAggresiveClone()
    {
        canApplyOnHitEffect = aggresiveCloneUnlockButton.unlocked;

        if (canApplyOnHitEffect)
            attackMultiplier = aggresiveCloneAttackMultiplier;
    }

    private void UnlockMultiClone()
    {
        canDuplicateClone = multipleUnlockButton.unlocked;

        if (canDuplicateClone)
            attackMultiplier = multiCloneAttackMultiplier;
    }

    private void UnlockCrystalInstead()
    {
        crystalInseadOfClone = crystalInseadUnlockButton.unlocked;
    }


    #endregion


    public void CreateClone(Transform _clonePosition,Vector3 _offset)
    {
        if (crystalInseadOfClone)
        {
            SkillManager.instance.crystal.CreateCrystal();
            return;
        }

        GameObject newClone = Instantiate(clonePrefab);

        newClone.GetComponent<Clone_Skill_Controller>().
            SetupClone(_clonePosition, cloneDuration, canAttack,_offset,FindClosestEnemy(newClone.transform),canDuplicateClone,chanceToDuplicate,player,attackMultiplier);
    }


    public void CreateCloneWithDelay(Transform _enemyTransform)
    {
        StartCoroutine(CloneDelayCorotine(_enemyTransform, new Vector3(2 * player.facingDir, 0)));
    }

    private IEnumerator CloneDelayCorotine(Transform _trasnform,Vector3 _offset)
    {
        yield return new WaitForSeconds(.4f);
            CreateClone(_trasnform,_offset);
    }
}
