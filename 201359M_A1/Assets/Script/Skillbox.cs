using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices;

[System.Serializable]
public class Skill
{
    public string name;
    public int level;
    //public int price;

    public Skill(string _name, int _level)
    {
        name = _name;
        level = _level;
    }
}
public class Skillbox : MonoBehaviour
{

    public TextMeshProUGUI skillname;
    public Button SkillLevelUpgradeButton;
    public TMP_Text skillleveltext;


    public int price;

   public Skill ReturnClass()
    {
        return new Skill(skillname.text, int.Parse(skillleveltext.text));
    }

    public void SetUI(Skill skill)
    {
        skillname.text = skill.name;
        skillleveltext.text = $"{skill.level}";

        //SkillLevelSlider.value = skill.level;
    }


    //public void SetUIWithoutJSON(Skill skill)
    //{
    //    skillname.text = skill.name;
    //    skillleveltext.text = $"{skill.level}";

    //    //SkillLevelSlider.value = skill.level;
    //}

    //public void SliderChangeUpdate(float num)
    //{
    //    skillleveltext.text = SkillLevelUpgradeButton.value.ToString();
    //}
}
