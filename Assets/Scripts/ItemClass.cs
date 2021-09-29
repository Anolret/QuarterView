using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ItemCode {
	public const int Main = 0;           //(대)분류
	public const int Sub = 1;           //(소)분류

	/// <summary>
	/// 소비템(대)
	/// </summary>
	public const int Expendables = 0;
	/// <summary>
	/// 물약(소)
	/// </summary>
	public const int Portion = 0;
	/// <summary>
	/// 버프(소)
	/// </summary>
	public const int Buff = 1;
	/// <summary>
	/// 퀘스트템(소)
	/// </summary>
	public const int Quest = 2;

	/// <summary>
	/// 장비템(대)
	/// </summary>
	public const int Equipment = 1;
	/// <summary>
	/// 무기(소)
	/// </summary>
	public const int Weapon = 0;
	/// <summary>
	/// 방어구(소)
	/// </summary>
	public const int Armor = 1;
	/// <summary>
	/// 장신구(소)
	/// </summary>
	public const int Accessory = 2;
}

public static class CodeE {
	//장비슬롯 크기
	public const int Size = 12;
	/// <summary>
	/// 투구
	/// </summary>
	public const int Helmet = 0;
	/// <summary>
	/// 갑옷
	/// </summary>
	public const int BodyArmor = 1;
	/// <summary>
	/// 장갑
	/// </summary>
	public const int Gloves = 2;
	/// <summary>
	/// 장화
	/// </summary>
	public const int Boots = 3;
	/// <summary>
	/// 목걸이
	/// </summary>
	public const int Amulet = 4;
	/// <summary>
	/// 반지1
	/// </summary>
	public const int OneRing = 5;
	/// <summary>
	/// 반지2
	/// </summary>
	public const int TwoRing = 6;
	/// <summary>
	/// 허리띠
	/// </summary>
	public const int Belt = 7;
	/// <summary>
	/// 주무기
	/// </summary>
	public const int Weapon = 8;
	/// <summary>
	/// 보조무기
	/// </summary>
	public const int SecondaryWeapon = 9;
	/// <summary>
	/// 물약1
	/// </summary>
	public const int OnePortion = 10;
	/// <summary>
	/// 물약2
	/// </summary>
	public const int TwoPortion = 11;
}

public enum CodeType {
	NULL = -1,
	Helmet = 0,
	BodyArmor,
	Gloves,
	Boots,
	Amulet,
	Ring,
	Belt = 7,
	Weapon,
	SWeapon,
	OnePortion,
	TwoPortion
}

public class Item {
	Sprite m_Img;
	string m_ItemName;
	int m_Amount;
	int[] m_Code = new int[2];
	bool m_Use;
	BaseStatus[] m_Status;
	CodeType m_CodeType;
	int m_Gold;

	public Item() {
		m_Img = null;
		m_ItemName = "null";
		m_Amount = -1;
		m_Code[0] = -1;
		m_Code[1] = -1;
		m_Use = false;
		m_Status = new BaseStatus[StatusConstant.MaxStatus];
		for (int i = 0; i < StatusConstant.MaxStatus; i++) {
			m_Status[i] = new BaseStatus();
		}
		m_CodeType = CodeType.NULL;
		m_Gold = 0;
	}
	public Item(Item _item) {
		m_Img = _item.GetImg();
		m_ItemName = _item.GetName();
		m_Amount = _item.GetAmount();
		m_Code[0] = _item.GetCode(0);
		m_Code[1] = _item.GetCode(1);
		m_Use = _item.GetUse();
		m_Status = _item.GetStaus();
		m_CodeType = _item.GetCodeType();
		m_Img = _item.GetImg();
		m_Gold = _item.GetGold();
	}

	public void SetImg(Sprite _img) { m_Img = _img; }
	public Sprite GetImg() { return m_Img; }

	public void SetItem(Item _item) {
		m_ItemName = _item.GetName();
		m_Amount = _item.GetAmount();
		m_Code[0] = _item.GetCode(0);
		m_Code[1] = _item.GetCode(1);
		m_Use = _item.GetUse();
		m_Status = _item.GetStaus();
		m_CodeType = _item.GetCodeType();
		m_Gold = _item.GetGold();
	}

	//이름
	public void SetName(string _name) { m_ItemName = _name; }
	public string GetName() { return m_ItemName; }

	//아이템갯수
	public void SetAmount(int _amount) { m_Amount = _amount; }
	public void AddAmount(int _amount) { m_Amount += _amount; }
	public int GetAmount() { return m_Amount; }
	public void ItemUes() { if (0 < m_Amount) m_Amount--; }

	//코드
	public void SetCode(int _num, int _set) { m_Code[_num] = _set; }
	public int[] GetCode() { return m_Code; }
	public int GetCode(int _num) { return m_Code[_num]; }
	public int GetMainCode() { return m_Code[0]; }
	public int GetSubCode() { return m_Code[1]; }
	public string GetCodeStr() { return "" + m_Code[ItemCode.Main] + m_Code[ItemCode.Sub]; }

	//사용가능여부
	public void SetUse(bool _use) { m_Use = _use; }
	public bool GetUse() { return m_Use; }

	//능력치
	public void SetStatus(BaseStatus[] _status) { m_Status = _status; }
	public void SetStatus(int _status, BaseStatus basestatus) { m_Status[_status] = basestatus; }
	public void AddStatus(int _status, float _value) { m_Status[_status].Set(_value); } //능력치 선택 및 설정
	public float GetStaus(int _status) { return m_Status[_status].GetBase(); }
	public BaseStatus[] GetStaus() { return m_Status; }

	//코드 타입
	public void SetCodeType(CodeType _codeType) { m_CodeType = _codeType; }
	public CodeType GetCodeType() { return m_CodeType; }

	//가격
	public void SetGold(int _gold) { m_Gold = _gold; }
	public int GetGold() { return m_Gold; }
}

//------------일반아이템
public class Consumable : Item {
	public Consumable() {
		SetCode(ItemCode.Main, ItemCode.Expendables);
	}
}

//물약
public class Portion : Consumable {
	public Portion(Item _item, string _sprite) {
		SetImg(ItemManager.Instance.m_ItemSprites[_sprite]);
		SetItem(_item);
	}
	public Portion(string _itemName, int _amount, CodeType _tType, int _selectStatus, float _value, int _gold, string _sprite, bool _use = true) {
		SetImg(ItemManager.Instance.m_ItemSprites[_sprite]);
		SetCode(ItemCode.Sub, ItemCode.Portion);
		SetName(_itemName);
		SetAmount(_amount);
		SetCodeType(_tType);
		AddStatus(_selectStatus, _value);
		SetUse(_use);
		SetGold(_gold);
	}

	public int PortionUes() {
		if (GetUse()) {
			ItemUes();
			return 0;
		}
		return 0;
	}
}
//버프
public class Buffitem : Consumable {
	public Buffitem() {}
	public Buffitem(Item _item) {
		SetImg(ItemManager.Instance.GetItemImg());
		SetItem(_item);
	}
	public Buffitem(Item _item, Sprite _sp) {
		SetImg(_sp);
		SetItem(_item);
	}
	public Buffitem(string _itemName, CodeType _type, bool _use = true) {
		SetCode(ItemCode.Sub, ItemCode.Buff);
		SetName(_itemName);
		SetAmount(1);
		SetCodeType(_type);
		SetUse(_use);
	}
	public Buffitem(string _itemName, int _amount, CodeType _type, bool _use = true) {
		SetCode(ItemCode.Sub, ItemCode.Buff);
		SetName(_itemName);
		SetAmount(_amount);
		SetCodeType(_type);
		SetUse(_use);
	}
}
//퀘스트
public class Quest : Consumable {
	public Quest() {}
	public Quest(Item _item) {
		SetImg(ItemManager.Instance.GetItemImg());
		SetItem(_item);
	}
	public Quest(Item _item, Sprite _sp) {
		SetImg(_sp);
		SetItem(_item);
	}
	public Quest(string _itemName, CodeType _type, bool _use = false) {
		SetCode(ItemCode.Sub, ItemCode.Quest);
		SetName(_itemName);
		SetAmount(1);
		SetUse(_use);
		SetCodeType(_type);
	}
	public Quest(string _itemName, int _amount, CodeType _type, bool _use = true) {
		SetCode(ItemCode.Sub, ItemCode.Quest);
		SetName(_itemName);
		SetAmount(_amount);
		SetUse(_use);
		SetCodeType(_type);
	}
}

//------------장비아이템
public class Equipment : Item {
	public Equipment() {
		SetCode(ItemCode.Main, ItemCode.Equipment);
	}
}
//무기
public class Weapon : Equipment {
	public Weapon(string _itemName, CodeType _type, string _sprite, bool _use = true) {
		SetImg(ItemManager.Instance.m_ItemSprites[_sprite]);
		SetCode(ItemCode.Sub, ItemCode.Weapon);
		SetName(_itemName);
		SetAmount(1);
		SetCodeType(_type);
		SetUse(_use);
	}
}

//방어구
public class Armor : Equipment {
	public Armor(string _itemName, CodeType _type, string _sprite, bool _use = true) {
		SetImg(ItemManager.Instance.m_ItemSprites[_sprite]);
		SetCode(ItemCode.Sub, ItemCode.Armor);
		SetName(_itemName);
		SetAmount(1);
		SetCodeType(_type);
		SetUse(_use);
	}
}

//장신구
public class Acc : Equipment {
	public Acc(string _itemName, CodeType _type, string _sprite, bool _use = true) {
		SetImg(ItemManager.Instance.m_ItemSprites[_sprite]);
		SetCode(ItemCode.Sub, ItemCode.Armor);
		SetName(_itemName);
		SetAmount(1);
		SetCodeType(_type);
		SetUse(_use);
	}
}