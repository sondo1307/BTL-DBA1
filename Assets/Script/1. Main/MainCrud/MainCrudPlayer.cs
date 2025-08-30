public class MainCrudPlayer: MainCrudObjectBase
{
    protected override void OnValidate()
    {
        base.OnValidate();
        // AddDataGob = GetComponentInChildren<UpdateAndInsertCauthuDataGrid>();
    }
}