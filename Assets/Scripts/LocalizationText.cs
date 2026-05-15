using System;
using System.Collections.Generic;
using UnityEngine;

public static class LocalizationText
{
    private static readonly Dictionary<string, string> translations = new Dictionary<string, string>
    {
        { "Souls", "灵魂" },
        { "Cost", "消耗" },
        { "Cost:", "消耗：" },
        { "Cooldown", "冷却中" },
        { "Cooldown!", "冷却中" },
        { "Inventory is full", "背包已满" },
        { "Unique", "特殊效果" },
        { "Unique:", "特殊效果：" },

        { "Weapon", "武器" },
        { "Armor", "护甲" },
        { "Amulet", "饰品" },
        { "Flask", "药瓶" },
        { "Material", "材料" },
        { "Equipment", "装备" },

        { "Strength", "力量" },
        { "Agility", "敏捷" },
        { "Intelligence", "智力" },
        { "Intelegence", "智力" },
        { "Vitality", "体质" },
        { "Damage", "伤害" },
        { "Crit.Chance", "暴击率" },
        { "Crit.Power", "暴击伤害" },
        { "Health", "生命" },
        { "Evasion", "闪避" },
        { "Magic Resist.", "魔法抗性" },
        { "Magic resist.", "魔法抗性" },
        { "Fire damage", "火焰伤害" },
        { "Ice damage", "冰霜伤害" },
        { "Lighting dmg.", "雷电伤害" },
        { "Lighting Damage", "雷电伤害" },

        { "Continue", "继续游戏" },
        { "New game", "新游戏" },
        { "Exit game", "退出游戏" },
        { "Options", "设置" },
        { "Inventory", "背包" },
        { "Character", "角色" },
        { "Skill tree", "技能树" },
        { "Craft", "制作" },
        { "CRAFT", "制作" },
        { "SAVE & EXIT", "保存并退出" },
        { "You died!", "你死了！" },
        { "Try again!", "再试一次！" },
        { "Major stats", "主要属性" },
        { "Offensive stats", "攻击属性" },
        { "Defensive stats", "防御属性" },
        { "Background music", "背景音乐" },
        { "Sound effect", "音效" },
        { "Show health bar avobe player", "显示玩家血条" },
        { "Show health bar above player", "显示玩家血条" },
        { "Sword throw", "飞剑" },

        { "Blackhole", "黑洞" },
        { "Mirage blink", "幻影闪现" },
        { "Bullet sword", "穿刺飞剑" },
        { "Crystal mirage", "水晶幻影" },
        { "Multiple distruction", "多重毁灭" },
        { "Sword Throw", "投掷飞剑" },
        { "Dash - \"Here I am\"", "冲刺 - 我在这里" },
        { "Dodge", "闪避" },
        { "Parry with a mirage", "幻影格挡" },
        { "Dash - \"Actually here I am\"", "冲刺 - 我其实在这" },
        { "Explosion", "爆裂" },
        { "Aggresive mirage", "进攻幻影" },
        { "Controlled destruction", "控制毁灭" },
        { "Vulnerability", "破绽" },
        { "Multiple mirage", "多重幻影" },
        { "Bouncy sword", "弹射飞剑" },
        { "Chain saw sword", "链锯飞剑" },
        { "Crystal", "水晶" },
        { "Dodge mirage", "闪避幻影" },
        { "Parry", "格挡" },
        { "Dash", "冲刺" },
        { "Time stop", "时间停止" },
        { "Time mirage", "时间幻影" },
        { "Restore with parry", "格挡恢复" },
        { "闪避 mirage", "闪避幻影" },

        { "You trap enemies in a blackhole which allows you to do tons of damage.", "将敌人困在黑洞中，让你可以造成大量伤害。" },
        { "You leave your mirage instead of you when you blink. [Only one crystal upgrade can be choosen]", "闪现时留下幻影代替你。\n[只能选择一个水晶强化]" },
        { "Sword can peirce targets. [Only one sword upgrade can be choose]", "飞剑可以穿透目标。\n[只能选择一个飞剑强化]" },
        { "You create crystal instead of mirage. With all it's qualites. [Only one upgrade can be choosen]", "生成水晶代替幻影，并保留它的全部特性。\n[只能选择一个强化]" },
        { "You can create up to 3 crystal at a time.", "你可以同时生成最多 3 个水晶。" },
        { "Throws sword into the enemy.", "向敌人投掷飞剑。" },
        { "You leave mirage when you dash.", "冲刺时留下幻影。" },
        { "You getting 10% of a evasion.", "获得 10% 闪避。" },
        { "You create a clone with succsesful parry.", "成功格挡时生成一个分身。" },
        { "You create mirage at your arrival.", "到达位置时生成幻影。" },
        { "Crystal can explode. [Only one crystal upgrade can be choosen]", "水晶可以爆炸。\n[只能选择一个水晶强化]" },
        { "Your mirage has 80% of your damage. Your mirage can apply on hit effects.", "你的幻影拥有你 80% 的伤害，并且可以触发命中效果。" },
        { "Crystal moves towards the enemy. You can no longer teleport to crystal.", "水晶会向敌人移动。\n你将不能再传送到水晶处。" },
        { "Enemy hit by sword , takes 10% more damage.", "被飞剑命中的敌人受到的伤害提高 10%。" },
        { "Your mirage has 30% of your attack. Your mirgae can spawn another mirage. [Only one upgrade can be choosen]", "你的幻影拥有你 30% 的攻击力，并且可以生成另一个幻影。\n[只能选择一个强化]" },
        { "Sword can boucne beetwen targets. [Only one sword upgrade can be choose]", "飞剑可以在目标之间弹射。\n[只能选择一个飞剑强化]" },
        { "Sword will spin like a chain saw. [Only one sword upgrade can be choose]", "飞剑会像链锯一样旋转。\n[只能选择一个飞剑强化]" },
        { "You create magic crystal that does magic damage. By clickinb ability again , you can teleport to your crystal.", "生成造成魔法伤害的魔法水晶。\n再次使用技能可以传送到水晶处。" },
        { "You can create mirage on dodge.", "闪避时可以生成幻影。" },
        { "You can parry attack to avoid damage.", "你可以格挡攻击来避免伤害。" },
        { "You can dash to avoid attacks. You are invincible in the time of dash.", "你可以冲刺来躲避攻击。\n冲刺期间你处于无敌状态。" },
        { "When enemy hit by sword, his time stops.", "敌人被飞剑命中时，时间会停止。" },
        { "Your mirage can attack targets. Your mirage has 30% of your damage.", "你的幻影可以攻击目标，并拥有你 30% 的伤害。" },
        { "You restore a bit of health with succsesful parry.", "成功格挡时恢复少量生命。" },

        { "Amulet of god", "神之护符" },
        { "Golden ring", "金戒指" },
        { "The book", "古书" },
        { "Blue wolf armor", "蓝狼护甲" },
        { "Plate armor", "板甲" },
        { "Rogue's jacket", "游侠夹克" },
        { "Armor flask", "护甲药瓶" },
        { "Fire flask", "火焰药瓶" },
        { "Healing flask", "治疗药瓶" },
        { "Iron Sword", "铁剑" },
        { "Animal skin", "兽皮" },
        { "Animal tooth", "兽牙" },
        { "Cloth", "布料" },
        { "Cotton", "棉花" },
        { "Diamond", "钻石" },
        { "Fairy dust", "妖精粉尘" },
        { "Feather", "羽毛" },
        { "Fire herb", "火焰草" },
        { "Glass", "玻璃" },
        { "Glue", "胶水" },
        { "Golden bar", "金锭" },
        { "Iron", "铁块" },
        { "Moon water", "月亮水" },
        { "Thread", "线" },
        { "Wood", "木材" },

        { "Gives 50 armor for 7 seconds.", "获得 50 点护甲，持续 7 秒。" },
        { "Increase evasion on each hit taken.", "每次受到攻击时提高闪避。" },
        { "Increases fire damage on hit.", "命中时提高火焰伤害。" },
        { "Freeze enemies when below 10% hp.", "生命低于 10% 时冻结敌人。" },
        { "Freeze enemies when your hp below 10%.", "你的生命低于 10% 时冻结敌人。" },
        { "Heal's you for 10% on hit.", "命中时恢复 10% 生命。" },
        { "Create Ice and Fire on 3rd attack.", "第 3 次攻击时生成冰与火。" },
        { "Increases ice damage on hit.", "命中时提高冰霜伤害。" },
        { "Create Thunder Strike on hit.", "命中时触发雷击。" },
        { "Heals for 30% of health. Cooldown 15 sec.", "恢复 30% 生命。冷却 15 秒。" },
    };

    public static string Translate(string english)
    {
        if (string.IsNullOrWhiteSpace(english))
            return english;

        string key = english.Trim();

        if (translations.TryGetValue(key, out string translated))
            return PreservePadding(english, translated);

        string normalizedKey = NormalizeKey(key);

        if (translations.TryGetValue(normalizedKey, out translated))
            return PreservePadding(english, translated);

        return english;
    }

    public static string EquipmentType(EquipmentType type)
    {
        return Translate(type.ToString());
    }

    public static string ItemType(ItemType type)
    {
        return Translate(type.ToString());
    }

    private static string PreservePadding(string source, string translated)
    {
        string prefix = source.StartsWith(" ") ? " " : "";
        string suffix = source.EndsWith(" ") ? " " : "";

        return prefix + translated + suffix;
    }

    private static string NormalizeKey(string text)
    {
        return string.Join(" ", text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries));
    }
}
