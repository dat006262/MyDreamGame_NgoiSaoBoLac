Cách sử dụng :
Kéo StatusAuthoring vào chủ sở hữu,
sau đó nhập các giá trị cơ bản tại Editor

--------------------------------------------------------------------------------------
Sau khi thêm vào nhận Component CharacterStat bao gồm BasicHealth, MaxHealth,....
BasicHealth là chỉ số máu cơ bản
MaxHealth là chỉ số máu tối đa( thay đổi dựa trên trang bị vào, hiệu ứng Debuff, Buff) được tính toán thông qua Buffer StatModify
StatModfy có các dạng-chỉ số càng bé độ ưu tiên càng cao
(
 Flat = 100,//cong
 PercentAdd = 200,//VD:value *(1+PercentAdd+PercentAdd)
 PercentMulti = 300//Vd value*(1+PercentMul)*(1+PercentMove)

)
----------------------------------------------------------------------------------------
Cách hoạt động.
Có 1 biến là isdirty trong thành phần CheckNeedCalculate. Nếu biến này là False thì sẽ ko tính toán, ngược lại nếu là true sẽ tính 
toán lại các chỉ số MaxHealth...
Ban đầu isdirty =false và lần chạy đầu tiên sẽ set MaxHealth = BasicHealth 
....
---------------------------------------------------------------------------------------- 
