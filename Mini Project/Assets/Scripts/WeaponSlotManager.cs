using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SG
{
	public class WeaponSlotManager : MonoBehaviour
	{
		WeaponHolderSlot leftHandSlot;
		WeaponHolderSlot rightHandSlot;

		DamageCollider leftHandDamageCollider;
		DamageCollider rightHandDamageCollider;
		public void Awake()
		{
			WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
			foreach (WeaponHolderSlot weaponSlot in weaponHolderSlots)
			{
				if(weaponSlot.isLeftHandSlot)
				{
					leftHandSlot = weaponSlot;
				}
				else if (weaponSlot.isRightHandSlot)
				{
					rightHandSlot = weaponSlot;
				}
			}
		}

		public void LoadWeaponOnSLot(WeaponItem weaponItem, bool isLeft)
		{
			if (isLeft)
			{
				leftHandSlot.LoadWeaponModel(weaponItem);
				LoadLeftWeaponDamageCollider();
			}
			else
			{
				rightHandSlot.LoadWeaponModel(weaponItem);
				LoadRightWeaponDamageCollider();
			}
		}

        #region Handle Weapon's Damage Collider

        private void LoadLeftWeaponDamageCollider()
        {
			leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

		private void LoadRightWeaponDamageCollider()
		{
			rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
		}

		public void OpenRightDamageCollider()
        {
			rightHandDamageCollider.EnableDamageCollider();
        }

		public void OpenLeftDamageCollider()
		{
			leftHandDamageCollider.EnableDamageCollider();
		}

		public void CloseRightDamageCollider()
		{
			rightHandDamageCollider.DisableDamageCollider();
		}

		public void CloseLeftDamageCollider()
		{
			leftHandDamageCollider.DisableDamageCollider();
		}

        #endregion
    }
}
