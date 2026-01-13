using Microsoft.AspNetCore.Mvc;

namespace RedShirt.Adventure.Realm.Attributes;

public class ProducesJsonAttribute() : ProducesAttribute("application/json");