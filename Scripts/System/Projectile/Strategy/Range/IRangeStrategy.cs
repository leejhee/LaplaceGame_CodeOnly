using System.Collections.Generic;

namespace Client
{
    //range 처리는 콜라이더를 만들어주는 데서 해야하지 않을까...?
    //조금 고민되는 상태라 아직 구현 보류함.
    public interface IRangeStrategy
    {
        public List<CharBase> GetTargetsInRange(CharBase origin);
    }
}