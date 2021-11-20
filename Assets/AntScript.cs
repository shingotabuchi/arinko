using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntScript : MonoBehaviour
{
    float AntSpeed = SettingsScript.speed;
    float viewRadius = SettingsScript.viewradius;
    float pheromoneViewRadius;
    float FOV = SettingsScript.fov;
    float PheromoneFOV;
    float PheromoneSearchResolution;
    float foodPickupRadius = 0.1f;
    float MoveProbability = 100;
    float RotateProbability = SettingsScript.kyodori;
    // float RotateProbability = 30;
    float PheromoneDropPerSec = SettingsScript.pheromonePerSec;
    // float PheromoneDropPerSec = 5;
    float FollowPhermoneProbability = SettingsScript.majimesa;
    // float FollowPhermoneProbability = 70;
    float ReturnAngle = 30;
    public int maxPhermoneCount;
    public bool FollowOld;
    bool MukiMukiKeisanMode = SettingsScript.mukimuki;
    float ScreenHalfWidth;
    float ScreenHalfHeight;
    float PheromoneDropTimer;
    GameObject HeadPoint;
    Transform targetFood;
    Transform homeTransform;
    public GameObject PheromoneToHome;
    float PheromoneToHomeIntensity;
    public GameObject PheromoneToFood;
    float PheromoneToFoodIntensity;
    float PheromoneIntesityDropRate = 1/SettingsScript.gensuirate;
    public GameObject Pheromones;
    public GameObject Canvas;
    bool isHoldingFood = false;
    public static bool isInit = true;
    public bool isinMenuMode;
    // Start is called before the first frame update
    void Start()
    {
        PheromoneToHomeIntensity = 1;
        PheromoneToFoodIntensity = 1;
        pheromoneViewRadius = viewRadius/2;
        PheromoneFOV = FOV;
        PheromoneSearchResolution = FOV/SettingsScript.bunkainou;

        Physics2D.queriesStartInColliders = false;
        transform.Rotate(new Vector3(0, 0, Random.Range(0,360)));
        HeadPoint = this.gameObject.transform.GetChild(0).gameObject;
        ScreenHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        ScreenHalfHeight = ScreenHalfWidth / Camera.main.aspect;
        PheromoneDropTimer = Time.time + 1/PheromoneDropPerSec;
        isInit = false;
    }

    void GetStats(){

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isInit&&!isinMenuMode){
            if(Input.GetKeyDown("space")){
                isInit = false;
            }
        }
        else{
            if(targetFood==null){
                if(homeTransform==null){
                    // 行くあてがない場合
                    if(Random.Range(0,100)<RotateProbability){
                        transform.Rotate(new Vector3(0, 0, Random.Range(-30,30)));
                    }
                    if(Random.Range(0,100)<MoveProbability){
                        transform.position += transform.up*Time.deltaTime*AntSpeed;
                    }
                    if(isHoldingFood){
                        FollowPheromoneToHome();
                        FindHome();
                    }
                    else{
                        FollowPheromoneToFood();
                        FindFood();
                    }
                }
                else{
                    // 家をみつけた場合
                    Vector2 dirToHome = homeTransform.position-transform.position;
                    if(Random.Range(0,100)<FollowPhermoneProbability)transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToHome)+Random.Range(-20,20)));
                    transform.position += transform.up*Time.deltaTime*AntSpeed;
                    if(Vector2.Distance(homeTransform.position,transform.position)<foodPickupRadius){
                        Destroy(HeadPoint.transform.GetChild(0).gameObject);
                        homeTransform = null;
                        isHoldingFood = false;
                        transform.Rotate(new Vector3(0, 0, Random.Range(180-ReturnAngle/2,180+ReturnAngle/2)));
                    }
                }
            }
            else{
                // 食べ物をみつけた場合
                Vector2 dirToFood = targetFood.position-transform.position;
                if(Random.Range(0,100)<FollowPhermoneProbability)transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToFood)+Random.Range(-20,20)));
                transform.position += transform.up*Time.deltaTime*AntSpeed;
                if(Vector2.Distance(targetFood.position,transform.position)<foodPickupRadius){
                    Destroy(targetFood.gameObject.GetComponent<Collider2D>());
                    QueenAnt.FoodCount++;
                    QueenAnt.spawnAnt = QueenAnt.FoodCount%10==0;
                    targetFood.position = HeadPoint.transform.position;
                    targetFood.parent = HeadPoint.transform;
                    isHoldingFood = true;
                    targetFood = null;
                    transform.Rotate(new Vector3(0, 0, Random.Range(180-ReturnAngle/2,180+ReturnAngle/2)));
                }
            }

            if(Time.time>PheromoneDropTimer){
                PheromoneDropTimer = Time.time + 1/PheromoneDropPerSec;
                GameObject pheromone;
                if(isHoldingFood){
                    pheromone = Instantiate(PheromoneToFood, transform.position, Quaternion.identity);
                    SpriteRenderer pheromoneSprite = pheromone.GetComponent<SpriteRenderer>();
                    Color pheromoneColor = pheromoneSprite.color;
                    pheromoneSprite.color = new Color (pheromoneColor.r, pheromoneColor.g, pheromoneColor.b, PheromoneToFoodIntensity); 
                    if(PheromoneToFoodIntensity>0)PheromoneToFoodIntensity -= PheromoneIntesityDropRate;
                }
                else{
                    pheromone = Instantiate(PheromoneToHome, transform.position, Quaternion.identity);
                    SpriteRenderer pheromoneSprite = pheromone.GetComponent<SpriteRenderer>();
                    Color pheromoneColor = pheromoneSprite.color;
                    pheromoneSprite.color = new Color (pheromoneColor.r, pheromoneColor.g, pheromoneColor.b, PheromoneToHomeIntensity);
                    if(PheromoneToHomeIntensity>0)PheromoneToHomeIntensity -= PheromoneIntesityDropRate;
                }
                // pheromone.transform.parent = Pheromones.transform;
            }
            CheckBoundaries();
        }
    }

    void FindHome(){
        // Collider2D[] HomeInCircle = Physics2D.OverlapCircleAll(HeadPoint.transform.position,viewRadius,1 << LayerMask.NameToLayer("Home"));
        // if(HomeInCircle.Length>0){
        //     Transform home = HomeInCircle[0].transform;
        //     Vector2 dirToHome = home.position-transform.position;
        //     if(Vector2.Angle(transform.up,dirToHome) < FOV/2){
        //         transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToHome)+Random.Range(-20,20)));
        //         homeTransform = home;
        //     }
        // }

        for(float i=0;i<=PheromoneFOV/2;i+=PheromoneSearchResolution){
            RaycastHit2D hit = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("Home"));
            RaycastHit2D hit1 = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,-i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("Home"));
            if(hit.collider!=null){
                homeTransform = hit.collider.transform;
                Vector2 dirToHome = homeTransform.position-transform.position;
                transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToHome)+Random.Range(-20,20)));
                Debug.DrawLine(HeadPoint.transform.position,hit.point,Color.red);
                PheromoneToHomeIntensity = 1;
                break;
            }
            else{
                Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,i) * transform.up*pheromoneViewRadius,Color.blue);
                if(hit1.collider!=null){
                    homeTransform = hit1.collider.transform;
                    Vector2 dirToHome = homeTransform.position-transform.position;
                    transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToHome)+Random.Range(-20,20)));
                    Debug.DrawLine(HeadPoint.transform.position,hit.point,Color.red);
                    PheromoneToHomeIntensity = 1;
                    break;
                }
                else{
                    Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,-i) * transform.up*pheromoneViewRadius,Color.blue);
                }
            }
        }
    }

    void FindFood(){
        // Collider2D[] FoodinCircle = Physics2D.OverlapCircleAll(HeadPoint.transform.position,viewRadius,1 << LayerMask.NameToLayer("FoodLayer"));
        // if(FoodinCircle.Length>0){
        //     Transform closestFood = FoodinCircle[Random.Range(0,FoodinCircle.Length)].transform;
        //     float minDist = Vector2.Distance(closestFood.transform.position,transform.position);
        //     foreach (Collider2D Food in FoodinCircle)
        //     {
        //         if(Vector2.Distance(Food.transform.position,transform.position)<minDist){
        //             minDist = Vector2.Distance(Food.transform.position,transform.position);
        //             closestFood = Food.transform;
        //         }
        //     }
        //     Vector2 dirToFood = closestFood.position-transform.position;
        //     if(Vector2.Angle(transform.up,dirToFood) < FOV/2){
        //         transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToFood)+Random.Range(-20,20)));
        //         closestFood.gameObject.layer = LayerMask.NameToLayer("TakenFoodLayer");
        //         targetFood = closestFood;
        //     }
        // }

        for(float i=0;i<=PheromoneFOV/2;i+=PheromoneSearchResolution){
            RaycastHit2D hit = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("FoodLayer"));
            RaycastHit2D hit1 = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,-i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("FoodLayer"));
            if(hit.collider!=null){
                targetFood = hit.collider.transform;
                Vector2 dirToFood = targetFood.position-transform.position;
                transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToFood)+Random.Range(-20,20)));
                targetFood.gameObject.layer = LayerMask.NameToLayer("TakenFoodLayer");
                Debug.DrawLine(HeadPoint.transform.position,hit.point,Color.red);
                PheromoneToFoodIntensity = 1;
                break;
            }
            else{
                Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,i) * transform.up*pheromoneViewRadius,Color.blue);
                if(hit1.collider!=null){
                    targetFood = hit1.collider.transform;
                    Vector2 dirToFood = targetFood.position-transform.position;
                    transform.Rotate(new Vector3(0, 0, Vector2.SignedAngle(transform.up,dirToFood)+Random.Range(-20,20)));
                    targetFood.gameObject.layer = LayerMask.NameToLayer("TakenFoodLayer");
                    Debug.DrawLine(HeadPoint.transform.position,hit1.point,Color.red);
                    PheromoneToFoodIntensity = 1;
                    break;
                }
                else{
                    Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,-i) * transform.up*pheromoneViewRadius,Color.blue);
                }
            }
        }
    }

    void FollowPheromoneToHome(){
        if(Random.Range(0,100)<FollowPhermoneProbability){
            Vector2 MeanDirection = transform.up;

            if(MukiMukiKeisanMode){
                Collider2D[] PheromoneToHomeinCircle = Physics2D.OverlapCircleAll(HeadPoint.transform.position,pheromoneViewRadius,1 << LayerMask.NameToLayer("PheromoneToHome"));
                if(PheromoneToHomeinCircle.Length>0){
                    int PhermoneCount = 0;
                    foreach(Collider2D pheromone in PheromoneToHomeinCircle){
                    
                        Transform pheromoneTransform = pheromone.transform;
                        Vector2 dirToPheromone = pheromoneTransform.position - transform.position;
                        SpriteRenderer pheromoneSprite = pheromoneTransform.gameObject.GetComponent<SpriteRenderer>();
                        Color pheromoneColor = pheromoneSprite.color;
                        float phermoneStrength = pheromoneColor.a;
                        if(Vector2.Angle(transform.up,dirToPheromone)<PheromoneFOV/2){
                            if(FollowOld){
                                MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*(1-phermoneStrength);
                            }
                            else{
                                MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*phermoneStrength;
                            }
                        }
                        PhermoneCount++;
                        if(PhermoneCount>=maxPhermoneCount){
                            break;
                        }
                    }
                    transform.Rotate(new Vector3(0, 0, Random.Range(0,Vector2.SignedAngle(transform.up,MeanDirection))));
                // transform.Rotate(new Vector3(0,0,Random.Range(-DevianceAngle,DevianceAngle)));
                }
                return;
            }

            for(float i=-PheromoneFOV/2;i<=PheromoneFOV/2;i+=PheromoneSearchResolution){
                RaycastHit2D hit = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("PheromoneToHome"));
                if(hit.collider!=null){
                    Transform pheromoneTransform = hit.collider.transform;
                    Vector2 dirToPheromone = pheromoneTransform.position - transform.position;
                    SpriteRenderer pheromoneSprite = pheromoneTransform.gameObject.GetComponent<SpriteRenderer>();
                    Color pheromoneColor = pheromoneSprite.color;
                    float phermoneStrength = pheromoneColor.a;
                    if(FollowOld){
                       MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*(1-phermoneStrength);
                    }
                    else{
                        MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*phermoneStrength;
                    }
                }
                else{
                    Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,i) * transform.up*pheromoneViewRadius,Color.blue);
                }
            }
            transform.Rotate(new Vector3(0, 0, Random.Range(0,Vector2.SignedAngle(transform.up,MeanDirection))));
        }
    }

    void FollowPheromoneToFood(){
        if(Random.Range(0,100)<FollowPhermoneProbability){
            Vector2 MeanDirection = transform.up;
            if(MukiMukiKeisanMode){
                Collider2D[] PheromoneToFoodinCircle = Physics2D.OverlapCircleAll(HeadPoint.transform.position,pheromoneViewRadius,1 << LayerMask.NameToLayer("PheromoneToFood"));
                if(PheromoneToFoodinCircle.Length>0){
                    int PhermoneCount = 0;
                    foreach(Collider2D pheromone in PheromoneToFoodinCircle){
                        Transform pheromoneTransform = pheromone.transform;
                        Vector2 dirToPheromone = pheromoneTransform.position - transform.position;
                        SpriteRenderer pheromoneSprite = pheromoneTransform.gameObject.GetComponent<SpriteRenderer>();
                        Color pheromoneColor = pheromoneSprite.color;
                        float phermoneStrength = pheromoneColor.a;
                        if(Vector2.Angle(transform.up,dirToPheromone)<PheromoneFOV/2){
                            if(FollowOld){
                                MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*(1-phermoneStrength);
                            }
                            else{
                                MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*phermoneStrength;
                            }
                        }
                        PhermoneCount++;
                        if(PhermoneCount>=maxPhermoneCount){
                            break;
                        }
                    }
                    transform.Rotate(new Vector3(0, 0, Random.Range(0,Vector2.SignedAngle(transform.up,MeanDirection))));
                    // transform.Rotate(new Vector3(0,0,Random.Range(-DevianceAngle,DevianceAngle)));
                }
                return;
            }

            for(float i=-PheromoneFOV/2;i<=PheromoneFOV/2;i+=PheromoneSearchResolution){
                RaycastHit2D hit = Physics2D.Raycast(HeadPoint.transform.position,Quaternion.Euler(0,0,i) * transform.up,pheromoneViewRadius,1 << LayerMask.NameToLayer("PheromoneToFood"));
                if(hit.collider!=null){
                    Transform pheromoneTransform = hit.collider.transform;
                    Vector2 dirToPheromone = pheromoneTransform.position - transform.position;
                    SpriteRenderer pheromoneSprite = pheromoneTransform.gameObject.GetComponent<SpriteRenderer>();
                    Color pheromoneColor = pheromoneSprite.color;
                    float phermoneStrength = pheromoneColor.a;
                    if(FollowOld){
                       MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*(1-phermoneStrength);
                    }
                    else{
                        MeanDirection = (Vector2)MeanDirection + (Vector2)dirToPheromone.normalized*phermoneStrength;
                    }
                }
                else{
                    Debug.DrawLine(HeadPoint.transform.position,HeadPoint.transform.position+Quaternion.Euler(0,0,i) * transform.up*pheromoneViewRadius,Color.blue);
                }
            }
            transform.Rotate(new Vector3(0, 0, Random.Range(0,Vector2.SignedAngle(transform.up,MeanDirection))));
        }
    }

    void CheckBoundaries(){
        if(HeadPoint.transform.position.x>=ScreenHalfWidth){
            if(transform.localEulerAngles.z>=270){
                transform.Rotate(new Vector3(0, 0, 360-transform.localEulerAngles.z + Random.Range(0,90)));
            }
            if(transform.localEulerAngles.z<270){
                transform.Rotate(new Vector3(0, 0, 180-transform.localEulerAngles.z - Random.Range(0,90)));
            }
        }
        if(HeadPoint.transform.position.x<=-ScreenHalfWidth){
            if(transform.localEulerAngles.z>=90){
                transform.Rotate(new Vector3(0, 0, 180-transform.localEulerAngles.z + Random.Range(0,90)));
            }
            if(transform.localEulerAngles.z<90){
                transform.Rotate(new Vector3(0, 0, -transform.localEulerAngles.z - Random.Range(0,90)));
            }
        }
        if(HeadPoint.transform.position.y>=ScreenHalfHeight){
            if(transform.localEulerAngles.z<270){
                transform.Rotate(new Vector3(0, 0, 90-transform.localEulerAngles.z + Random.Range(0,90)));
            }
            if(transform.localEulerAngles.z>=270){
                transform.Rotate(new Vector3(0, 0, 270-transform.localEulerAngles.z - Random.Range(0,90)));
            }
        }
        if(HeadPoint.transform.position.y<=-ScreenHalfHeight){
            if(transform.localEulerAngles.z>=180){
                transform.Rotate(new Vector3(0, 0, 270-transform.localEulerAngles.z + Random.Range(0,90)));
            }
            if(transform.localEulerAngles.z<180){
                transform.Rotate(new Vector3(0, 0, 90-transform.localEulerAngles.z - Random.Range(0,90)));
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.gameObject.tag=="Food"&&isHoldingFood){
            PheromoneToFoodIntensity = 1;
            transform.Rotate(new Vector3(0, 0, Random.Range(180-ReturnAngle/2,180+ReturnAngle/2)));
            // if(!isHoldingFood){
            //     if(targetFood!=null){
            //         targetFood.gameObject.layer = LayerMask.NameToLayer("FoodLayer");
            //     }
            //     targetFood = collider.transform;
            // }
            // else transform.Rotate(new Vector3(0, 0, Random.Range(180-ReturnAngle/2,180+ReturnAngle/2)));
        }
        if(collider.gameObject.tag=="Home"&&!isHoldingFood){
            PheromoneToHomeIntensity = 1;
            transform.Rotate(new Vector3(0, 0, Random.Range(180-ReturnAngle/2,180+ReturnAngle/2)));
        }
    }
}
