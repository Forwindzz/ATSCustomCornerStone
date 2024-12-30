
# 1.1.0

Add Two conerstone:
- Overdraft Technical Contract: Get the orange cornerstone in advance... but then you need to pay for the price
- Volatile Market: The value of amber fluctuates when you trade with merchant.

Balance and design changes:
- Garden Design: Now there is no need for population (2 villagers is almost equivalent to no villager requirement, so I just deleted it). The negative effect is to reduce the hearth range by 2 (it should be considered a negative effect...? huh)
- Random numbers: Random generators will now help you avoid extremely bad luck

Code:
- Migrate to API interface v3.3.0
	- `CompositeEffectBuilder`
	- uuid generator for enum values
- Better compatibility
	- `MB.Settings` value modification should be changed in relative way as much as possible

添加了2种基石
- 预支技术协议 （提前拿橙色基石，那么代价是什么？）
- 市场波动 （琥珀价值会随着交易波动）

平衡性与设计调整：
- 园林景观设计：现在完全不需要人数（2人约等于没有，干脆删了），负面效果为缩小火塘范围2格（应该算是负面效果emmmm...）
- 随机数：随机数现在会一定程度让你避免运气过于不好的情况

代码：
- 迁移到API接口v3.3.0
	- `CompositeEffectBuilder`
	- uuid generator for enum values
- 更好的兼容性
	- `MB.Settings`数值修改尽量以相对方式更改

# 1.0.0

支持中文和英文！
Support Chinese and English!
\_(:з」∠)\_

添加4种基石。
- 沙拉食谱 （可以吃草了！）
- 盲目的赌徒 （总督沉迷盲盒不可自拔）
- 园林景观设计 （降低火塘升级需求）
- 可用性设计 （装饰提供加成）

Add 4 cornerstones.
- Salad Recipe (Now you can eat non-ediable goods!)
- Foolhardy Gambler (We need more blind boxes)
- Garden Design (Reduce hearth upgrade requirement)
- Usability Design (Decorations provide extra buff)


代码方面：
Code Aspect:
- Custom Save system
- Custom Hook Monitor system
- Custom service system
- Custom `CompositeEffectBuilder`
- Custom effects (may break compatibility, if other mods also modify this)
	- Modify `GoodModel.eatable` value
	- Modify Trader cornerstone price (with lots of patch)
	- Modify Hearth upgrade requirement
	- Modify Decoration points.