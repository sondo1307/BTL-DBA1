public class MainCrudReferee: MainCrudObjectBase
{
    protected override void OnValidate()
    {
        base.OnValidate();
        // AddDataGob = GetComponentInChildren<UpdateAndInsertTrongTaiDataGrid>();
    }
}